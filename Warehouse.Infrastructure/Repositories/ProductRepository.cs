using Warehouse.Domain.Models;
using Warehouse.Domain.Repositories;
using Warehouse.Infrastructure.Data;

namespace Warehouse.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    public Task<List<Product>> GetAllAsync()
    {
        return Task.FromResult(FakeWarehouseStore.Products);
    }

    public Task<Product?> GetByIdAsync(string id)
    {
        Product? product = FakeWarehouseStore.Products
            .FirstOrDefault(p => p.Id == id);

        return Task.FromResult(product);
    }

    public Task<List<Product>> SearchAsync(string? name, string? supplier)
    {
        IEnumerable<Product> products = FakeWarehouseStore.Products;

        if (!string.IsNullOrWhiteSpace(name))
        {
            products = products.Where(p =>
                p.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(supplier))
        {
            products = products.Where(p =>
                p.SupplierName.Contains(supplier, StringComparison.OrdinalIgnoreCase));
        }

        return Task.FromResult(products.ToList());
    }

    public Task AddAsync(Product product)
    {
        FakeWarehouseStore.Products.Add(product);

        return Task.CompletedTask;
    }

    public Task UpdateAsync(Product product)
    {
        return Task.CompletedTask;
    }

    public Task<bool> SkuExistsAsync(string sku)
    {
        bool exists = FakeWarehouseStore.Products.Any(p =>
            p.SKU.Equals(sku, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(exists);
    }
}