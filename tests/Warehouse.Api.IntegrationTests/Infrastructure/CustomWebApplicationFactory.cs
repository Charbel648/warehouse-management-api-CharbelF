using Warehouse.Domain.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Warehouse.Application.Common.Auth;
using Warehouse.Application.Common.Messaging;
using Warehouse.Application.Common.Storage;

namespace Warehouse.Api.IntegrationTests.Infrastructure;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            var testConfig = new Dictionary<string, string?>
            {
                ["Firebase:ProjectId"] = "warehouse-management-api-cf",

                ["Redis:ConnectionString"] = "localhost:6379",
                ["ConnectionStrings:Redis"] = "localhost:6379",

                ["Minio:Endpoint"] = "localhost:9000",
                ["Minio:AccessKey"] = "test-access-key",
                ["Minio:SecretKey"] = "test-secret-key",
                ["Minio:BucketName"] = "warehouse-assets",
                ["Minio:UseSsl"] = "false",

                ["RabbitMq:HostName"] = "localhost",
                ["RabbitMq:Port"] = "5672",
                ["RabbitMq:UserName"] = "warehouse",
                ["RabbitMq:Password"] = "warehouse",
                ["RabbitMq:ExchangeName"] = "warehouse.events"
            };

            config.AddInMemoryCollection(testConfig);
        });

        builder.ConfigureTestServices(services =>
        {

            services.RemoveAll<IProductRepository>();
            services.RemoveAll<ISupplierRepository>();
            services.RemoveAll<IWarehouseFileRepository>();

            services.AddSingleton<TestWarehouseStore>();
            services.AddScoped<IProductRepository, TestProductRepository>();
            services.AddScoped<ISupplierRepository, TestSupplierRepository>();
            services.AddScoped<IWarehouseFileRepository, TestWarehouseFileRepository>();
            services.RemoveAll<IDistributedCache>();
            services.AddDistributedMemoryCache();

            services.RemoveAll<IObjectStorageService>();
            services.AddSingleton<IObjectStorageService, TestObjectStorageService>();

            services.RemoveAll<IWarehouseEventPublisher>();
            services.AddSingleton<IWarehouseEventPublisher, NoOpWarehouseEventPublisher>();

            services.RemoveAll<ICurrentUserService>();
            services.AddSingleton<ICurrentUserService, TestCurrentUserService>();

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                    options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
                    options.DefaultScheme = TestAuthHandler.SchemeName;
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    TestAuthHandler.SchemeName,
                    options => { });
        });
    }
}

