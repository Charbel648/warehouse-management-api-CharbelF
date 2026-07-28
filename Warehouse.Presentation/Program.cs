using Warehouse.Presentation.Middleware;
using System.Text.Json;
using System.Security.Cryptography.X509Certificates;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using Warehouse.Presentation.Services;
using Warehouse.Application.Common.Auth;
using Warehouse.Infrastructure.Storage;
using Warehouse.Application.Common.Storage;
using Minio;
using Warehouse.Infrastructure.Caching;
using Warehouse.Application.Common.Caching;
using Warehouse.Infrastructure.Messaging;
using Warehouse.Application.Common.Messaging;
using Microsoft.EntityFrameworkCore;
using Warehouse.Application.Mapping;
using Warehouse.Application.Products.Commands.CreateProduct;
using Warehouse.Domain.Repositories;
using Warehouse.Infrastructure.Persistence;
using Warehouse.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLocalization();

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

string firebaseProjectId =
    builder.Configuration["Firebase:ProjectId"]
    ?? throw new InvalidOperationException("Firebase project id is missing");


string redisConnectionString =
    builder.Configuration.GetConnectionString("Redis")
    ?? builder.Configuration["Redis:ConnectionString"]
    ?? throw new InvalidOperationException("Redis connection string is missing");


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<WarehouseDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("WarehouseDb"));
});

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<IProductReportRepository, ProductReportRepository>();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CreateProductCommand).Assembly);
});

builder.Services.AddAutoMapper(config =>
{
    config.AddProfile<WarehouseProfile>();
});


builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMq"));
builder.Services.AddSingleton<IWarehouseEventPublisher, RabbitMqWarehouseEventPublisher>();

builder.Services.AddScoped<ICacheService, RedisCacheService>();
builder.Services.AddScoped<IInventoryDashboardRepository, InventoryDashboardRepository>();
builder.Services.AddScoped<IWarehouseFileRepository, WarehouseFileRepository>();


builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
});

builder.Services.AddSingleton<IMinioClient>(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();

    string endpoint =
    configuration["Minio:Endpoint"]
    ?? throw new InvalidOperationException("MinIO endpoint is missing");

    string accessKey =
    configuration["Minio:AccessKey"]
    ?? throw new InvalidOperationException("MinIO access key is missing");

    string secretKey =
    configuration["Minio:SecretKey"]
    ?? throw new InvalidOperationException("MinIO secret key is missing");
    bool useSsl = bool.TryParse(configuration["Minio:UseSsl"], out bool parsedUseSsl) && parsedUseSsl;

    return new MinioClient()
        .WithEndpoint(endpoint)
        .WithCredentials(accessKey, secretKey)
        .WithSSL(useSsl)
        .Build();
});

builder.Services.AddScoped<IObjectStorageService, MinioObjectStorageService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, HttpCurrentUserService>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = $"https://securetoken.google.com/{firebaseProjectId}",

            ValidateAudience = true,
            ValidAudience = firebaseProjectId,

            ValidateLifetime = true,
            RequireExpirationTime = true,
            RequireSignedTokens = true,

            RoleClaimType = "role",

            IssuerSigningKeyResolver = (token, securityToken, kid, validationParameters) =>
            {
                using var httpClient = new HttpClient();

                string json = httpClient
                    .GetStringAsync("https://www.googleapis.com/robot/v1/metadata/x509/securetoken@system.gserviceaccount.com")
                    .GetAwaiter()
                    .GetResult();

                Dictionary<string, string> certificates =
                    JsonSerializer.Deserialize<Dictionary<string, string>>(json)
                    ?? new Dictionary<string, string>();

                if (!certificates.TryGetValue(kid, out string? certificateText))
                {
                    return Array.Empty<SecurityKey>();
                }

                X509Certificate2 certificate = X509Certificate2.CreateFromPem(certificateText);

                return new[]
                {
                    new X509SecurityKey(certificate)
                };
            }
        };
    });


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("WarehouseReader", policy =>
        policy.RequireAuthenticatedUser()
              .RequireClaim("role", "admin", "user"));

    options.AddPolicy("WarehouseWriter", policy =>
        policy.RequireAuthenticatedUser()
              .RequireClaim("role", "admin"));

    options.AddPolicy("WarehouseAdmin", policy =>
        policy.RequireAuthenticatedUser()
              .RequireClaim("role", "admin"));

    options.AddPolicy("AdminOnly", policy =>
        policy.RequireAuthenticatedUser()
              .RequireClaim("role", "admin"));

    options.AddPolicy("UserOnly", policy =>
        policy.RequireAuthenticatedUser()
              .RequireClaim("role", "user"));

    options.AddPolicy("AdminOrUser", policy =>
        policy.RequireAuthenticatedUser()
              .RequireClaim("role", "admin", "user"));

    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter Firebase ID token only. Do not write Bearer manually."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }



