using Warehouse.Application.Products.Commands;
using Warehouse.Application.Products.Queries;
using Warehouse.Application.Suppliers.Commands;
using Warehouse.Application.Suppliers.Queries;
using Warehouse.Domain.Repositories;
using Warehouse.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IProductRepository, ProductRepository>();
builder.Services.AddSingleton<ISupplierRepository, SupplierRepository>();

builder.Services.AddScoped<ListProducts>();
builder.Services.AddScoped<GetProductById>();
builder.Services.AddScoped<SearchProducts>();
builder.Services.AddScoped<CreateProduct>();
builder.Services.AddScoped<UpdateProductQuantity>();
builder.Services.AddScoped<UpdateProductPrice>();
builder.Services.AddScoped<ArchiveProduct>();
builder.Services.AddScoped<AssignSupplierToProduct>();
builder.Services.AddScoped<AddProductImage>();

builder.Services.AddScoped<ListSuppliers>();
builder.Services.AddScoped<GetSupplierById>();
builder.Services.AddScoped<CreateSupplier>();
builder.Services.AddScoped<DeactivateSupplier>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();

app.MapControllers();

app.Run();