using System.IdentityModel.Tokens.Jwt;
using System.Globalization;
using System.Security.Claims;
using Hangfire;
using Hangfire.MemoryStorage;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.OpenApi.Models;
using Warehouse.Presentation.Services;
using Warehouse.Infrastructure.Storage;
using Warehouse.Application.Common.Storage;
using Warehouse.Application.Common.Auth;
using Minio;
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
using Warehouse.Presentation.Security;
using Warehouse.Presentation.Swagger;
using System.Security.Cryptography.X509Certificates;


var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File(
            "logs/warehouse-api-.log",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 14);
});

string? warehouseConnectionString = builder.Configuration.GetConnectionString("WarehouseDb");
string? redisConnectionString = builder.Configuration.GetConnectionString("Redis");
string firebaseProjectId = builder.Configuration["Firebase:ProjectId"] ?? string.Empty;
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

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers(options =>
{
    options.Filters.AddService<ModelValidationFilter>();
    options.Filters.AddService<ActionLoggingFilter>();
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<AcceptLanguageHeaderOperationFilter>();
    options.OperationFilter<FirebaseBearerSecurityOperationFilter>();

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Paste Firebase ID token only. Do not write Bearer because Swagger adds it automatically.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
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

if (string.IsNullOrWhiteSpace(firebaseProjectId))
{
    throw new InvalidOperationException("Firebase ProjectId is missing.");
}

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        string issuer = $"https://securetoken.google.com/{firebaseProjectId}";
        string firebaseKeysUrl = "https://www.googleapis.com/robot/v1/metadata/x509/securetoken@system.gserviceaccount.com";

        options.RequireHttpsMetadata = true;
        options.IncludeErrorDetails = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,

            ValidateAudience = true,
            ValidAudience = firebaseProjectId,

            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            RoleClaimType = "role",
            NameClaimType = "user_id",

            IssuerSigningKeyResolver = (token, securityToken, kid, validationParameters) =>
            {
                if (string.IsNullOrWhiteSpace(kid))
                {
                    return Array.Empty<SecurityKey>();
                }

                using HttpClient httpClient = new HttpClient();

                string json = httpClient
                    .GetStringAsync(firebaseKeysUrl)
                    .GetAwaiter()
                    .GetResult();

                Dictionary<string, string>? certificates =
                    System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);

                if (certificates == null)
                {
                    return Array.Empty<SecurityKey>();
                }

                if (!certificates.TryGetValue(kid, out string? certificatePem))
                {
                    return Array.Empty<SecurityKey>();
                }

                X509Certificate2 certificate = X509Certificate2.CreateFromPem(certificatePem);

                return new[]
                {
                    new X509SecurityKey(certificate)
                    {
                        KeyId = kid
                    }
                };
            },

            ClockSkew = TimeSpan.FromMinutes(5)
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                ClaimsIdentity? identity = context.Principal?.Identity as ClaimsIdentity;

                string? firebaseUid = context.Principal?.FindFirstValue("user_id")
                                      ?? context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier)
                                      ?? context.Principal?.FindFirstValue("sub");

                if (!string.IsNullOrWhiteSpace(firebaseUid)
                    && identity != null
                    && !identity.HasClaim(claim => claim.Type == "firebase_uid"))
                {
                    identity.AddClaim(new Claim("firebase_uid", firebaseUid));
                }

                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(WarehousePolicies.WarehouseReader, policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireAssertion(context =>
            context.User.HasClaim("role", WarehouseRoles.Admin)
            || context.User.HasClaim("role", WarehouseRoles.User)
            || context.User.HasClaim(ClaimTypes.Role, WarehouseRoles.Admin)
            || context.User.HasClaim(ClaimTypes.Role, WarehouseRoles.User)
            || context.User.IsInRole(WarehouseRoles.Admin)
            || context.User.IsInRole(WarehouseRoles.User));
    });

    options.AddPolicy(WarehousePolicies.WarehouseAdmin, policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireAssertion(context =>
            context.User.HasClaim("role", WarehouseRoles.Admin)
            || context.User.HasClaim(ClaimTypes.Role, WarehouseRoles.Admin)
            || context.User.IsInRole(WarehouseRoles.Admin));
    });
});
builder.Services.AddSingleton<IMinioClient>(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();

    string endpoint = configuration["Minio:Endpoint"] ?? "localhost:9000";
    string accessKey = configuration["Minio:AccessKey"] ?? throw new InvalidOperationException("MinIO access key is missing");
    string secretKey = configuration["Minio:SecretKey"] ?? throw new InvalidOperationException("MinIO secret key is missing");
    bool useSsl = bool.TryParse(configuration["Minio:UseSsl"], out bool parsedUseSsl) && parsedUseSsl;

    return new MinioClient()
        .WithEndpoint(endpoint)
        .WithCredentials(accessKey, secretKey)
        .WithSSL(useSsl)
        .Build();
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
builder.Services.AddScoped<IObjectStorageService, MinioObjectStorageService>();
builder.Services.AddScoped<IWarehouseFileRepository, WarehouseFileRepository>();
builder.Services.AddScoped<ICurrentUserService, HttpCurrentUserService>();

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

app.UseAuthentication();
app.UseAuthorization();

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
    job => job.CheckExpiringProductsAsync(CancellationToken.None),
    app.Configuration["BackgroundJobs:ExpiredProductsCron"] ?? Cron.Daily());

try
{
    app.Run();
}
finally
{
    Log.CloseAndFlush();
}








