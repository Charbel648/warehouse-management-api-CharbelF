using System.Globalization;
using Hangfire;
using Hangfire.MemoryStorage;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Warehouse.Application.BackgroundJobs;
using Warehouse.Application.Common.Caching;
using Warehouse.Application.Mapping;
using Warehouse.Application.Products.Commands.CreateProduct;
using Warehouse.Domain.Repositories;
using Warehouse.Infrastructure.Caching;
using Warehouse.Infrastructure.Persistence;
using Warehouse.Infrastructure.Repositories;
using Warehouse.Presentation.Filters;
using Warehouse.Presentation.Middleware;
using Warehouse.Presentation.Swagger;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        "logs/warehouse-api-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 14)
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    string? warehouseConnectionString = builder.Configuration.GetConnectionString("WarehouseDb");
    string? redisConnectionString = builder.Configuration.GetConnectionString("Redis");

    builder.Services.Configure<ApiBehaviorOptions>(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });

    builder.Services.AddLocalization(options =>
    {
        options.ResourcesPath = "Resources";
    });

    builder.Services.Configure<RequestLocalizationOptions>(options =>
    {
        var supportedCultures = new[]
        {
            new CultureInfo("en-US"),
            new CultureInfo("fr-FR")
        };

        options.DefaultRequestCulture = new RequestCulture("en-US");
        options.SupportedCultures = supportedCultures;
        options.SupportedUICultures = supportedCultures;

        options.RequestCultureProviders = new List<IRequestCultureProvider>
        {
            new AcceptLanguageHeaderRequestCultureProvider(),
            new QueryStringRequestCultureProvider()
        };
    });

    builder.Services.AddControllers(options =>
    {
        options.Filters.AddService<ModelValidationFilter>();
        options.Filters.AddService<ActionLoggingFilter>();
    });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.OperationFilter<AcceptLanguageHeaderOperationFilter>();
    });

    builder.Services.AddDbContext<WarehouseDbContext>(options =>
    {
        options.UseNpgsql(warehouseConnectionString);
    });

    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisConnectionString;
        options.InstanceName = "WarehouseApi_";
    });

    builder.Services.AddHealthChecks()
        .AddNpgSql(
            warehouseConnectionString ?? string.Empty,
            name: "postgresql-database")
        .AddRedis(
            redisConnectionString ?? "localhost:6379",
            name: "redis-cache");

    builder.Services.AddHealthChecksUI(options =>
    {
        options.AddHealthCheckEndpoint("Warehouse API", "/health");
    })
    .AddInMemoryStorage();

    builder.Services.AddHangfire(configuration =>
    {
        configuration.UseMemoryStorage();
    });

    builder.Services.AddHangfireServer();

    builder.Services.AddScoped<IProductRepository, ProductRepository>();
    builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
    builder.Services.AddScoped<IProductReportRepository, ProductReportRepository>();
    builder.Services.AddScoped<IInventoryDashboardRepository, InventoryDashboardRepository>();
    builder.Services.AddScoped<ICacheService, RedisCacheService>();

    builder.Services.AddScoped<ModelValidationFilter>();
    builder.Services.AddScoped<ActionLoggingFilter>();

    builder.Services.AddScoped<ExpiringProductsBackgroundJob>();

    builder.Services.AddMediatR(cfg =>
    {
        cfg.RegisterServicesFromAssembly(typeof(CreateProductCommand).Assembly);
    });

    builder.Services.AddAutoMapper(config =>
    {
        config.AddProfile<WarehouseProfile>();
    });

    var app = builder.Build();

    app.UseRequestLocalization();

    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.UseMiddleware<CorrelationIdMiddleware>();
    app.UseMiddleware<RequestTimingMiddleware>();

    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseStaticFiles();

    app.MapControllers();

    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    app.MapHealthChecksUI(options =>
    {
        options.UIPath = "/health-ui";
        options.ApiPath = "/health-ui-api";
    });

    app.UseHangfireDashboard("/hangfire");

    RecurringJob.AddOrUpdate<ExpiringProductsBackgroundJob>(
        "expired-and-expiring-products-check",
        job => job.CheckExpiringProductsAsync(),
        app.Configuration["BackgroundJobs:ExpiredProductsCron"] ?? Cron.Daily);

    app.Run();
}
catch (Exception exception)
{
    Log.Fatal(exception, "Application failed to start");
}
finally
{
    Log.CloseAndFlush();
}
