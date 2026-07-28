using Warehouse.Domain.Models;
using Warehouse.Domain.Repositories;

namespace Warehouse.Api.IntegrationTests.Infrastructure;

public class TestProductRepository : IProductRepository
{
    private readonly TestWarehouseStore _store;

    public TestProductRepository(TestWarehouseStore store)
    {
        _store = store;
    }

    public Task<List<Product>> GetAllAsync()
    {
        return Task.FromResult(_store.Products.ToList());
    }

    public Task<Product?> GetByIdAsync(string id)
    {
        return Task.FromResult(_store.Products.FirstOrDefault(product => product.Id == id));
    }

    public Task<List<Product>> SearchAsync(string? name, string? supplier)
    {
        IEnumerable<Product> query = _store.Products;

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(product =>
                product.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(supplier))
        {
            query = query.Where(product =>
                product.SupplierName.Contains(supplier, StringComparison.OrdinalIgnoreCase));
        }

        return Task.FromResult(query.ToList());
    }

    public Task AddAsync(Product product)
    {
        _store.Products.Add(product);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Product product)
    {
        return Task.CompletedTask;
    }

    public Task<bool> SkuExistsAsync(string sku)
    {
        bool exists = _store.Products.Any(product =>
            product.SKU.Equals(sku, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(exists);
    }

    public Task<List<Product>> GetExpiredOrExpiringProductsAsync(
        DateTime currentDate,
        DateTime expiringLimitDate,
        CancellationToken cancellationToken)
    {
        var products = _store.Products
            .Where(product => product.ExpiryDate <= expiringLimitDate)
            .ToList();

        return Task.FromResult(products);
    }
}
