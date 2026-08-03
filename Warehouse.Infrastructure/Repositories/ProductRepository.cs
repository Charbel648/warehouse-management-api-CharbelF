using Microsoft.EntityFrameworkCore;
using Warehouse.Domain.Models;
using Warehouse.Domain.Repositories;
using Warehouse.Infrastructure.Persistence;

namespace Warehouse.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly WarehouseDbContext _context;

    public ProductRepository(WarehouseDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetAllAsync()
    {
        return await _context.Products
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(string id)
    {
        return await _context.Products
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.ProductId == id);
    }

    public async Task<List<Product>> SearchAsync(string? name, string? supplier)
    {
        IQueryable<Product> query = _context.Products;

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(p =>
                EF.Functions.ILike(p.Name, $"%{name}%"));
        }

        if (!string.IsNullOrWhiteSpace(supplier))
        {
            query = query.Where(p =>
                EF.Functions.ILike(p.SupplierName, $"%{supplier}%"));
        }

        return await query.ToListAsync();
    }

    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> SkuExistsAsync(string sku)
    {
        return await _context.Products
            .AnyAsync(p => EF.Functions.ILike(p.SKU, sku));
    }

    public async Task<List<Product>> GetExpiredOrExpiringProductsAsync(
        DateTime currentDate,
        DateTime expiringLimitDate,
        CancellationToken cancellationToken)
    {
        return await _context.Products
            .Where(product =>
                !product.IsArchived
                && product.ExpiryDate <= expiringLimitDate)
            .OrderBy(product => product.ExpiryDate)
            .ToListAsync(cancellationToken);
    }
}

