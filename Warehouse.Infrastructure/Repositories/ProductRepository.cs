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
            .Include(p => p.Supplier)
            .Include(p => p.Images)
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(string id)
    {
        return await _context.Products
            .Include(p => p.Supplier)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.ProductId == id);
    }

    public async Task<List<Product>> SearchAsync(string? name, string? supplier)
    {
        IQueryable<Product> query = _context.Products
            .Include(p => p.Supplier)
            .Include(p => p.Images);

        if (!string.IsNullOrWhiteSpace(name))
        {
            string loweredName = name.ToLower();

            query = query.Where(p =>
                p.Name.ToLower().Contains(loweredName));
        }

        if (!string.IsNullOrWhiteSpace(supplier))
        {
            string loweredSupplier = supplier.ToLower();

            query = query.Where(p =>
                p.SupplierName.ToLower().Contains(loweredSupplier)
                || (p.Supplier != null && p.Supplier.Name.ToLower().Contains(loweredSupplier)));
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
            .AnyAsync(p => p.SKU.ToLower() == sku.ToLower());
    }
}
