using Microsoft.EntityFrameworkCore;
using Warehouse.Application.Mapping;
using Warehouse.Application.Products.Commands.CreateProduct;
using Warehouse.Domain.Repositories;
using Warehouse.Infrastructure.Persistence;
using Warehouse.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

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

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();

app.MapControllers();

app.Run();

