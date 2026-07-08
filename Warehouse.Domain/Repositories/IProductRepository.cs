using Warehouse.Domain.Models;

namespace Warehouse.Domain.Repositories;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();

    Task<Product?> GetByIdAsync(string id);

    Task<List<Product>> SearchAsync(string? name, string? supplier);

    Task AddAsync(Product product);

    Task UpdateAsync(Product product);

    Task<bool> SkuExistsAsync(string sku);
}