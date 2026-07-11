using Microsoft.EntityFrameworkCore;
using Warehouse.Domain.Models;
using Warehouse.Domain.Repositories;
using Warehouse.Infrastructure.Persistence;

namespace Warehouse.Infrastructure.Repositories;

public class ProductReportRepository : IProductReportRepository
{
    private readonly WarehouseDbContext _context;

    public ProductReportRepository(WarehouseDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetProductsBySupplierAsync(string supplierName, string sortOrder)
    {
        IQueryable<Product> query = _context.Products
            .Include(p => p.Supplier)
            .Include(p => p.Images)
            .Where(p =>
                p.SupplierName.ToLower() == supplierName.ToLower()
                || (p.Supplier != null && p.Supplier.Name.ToLower() == supplierName.ToLower()));

        if (sortOrder.ToLower() == "asc")
        {
            query = query.OrderBy(p => p.CreatedAt);
        }
        else
        {
            query = query.OrderByDescending(p => p.CreatedAt);
        }

        return await query.ToListAsync();
    }

    public async Task<int> GetTotalProductsCountAsync()
    {
        return await _context.Products.CountAsync();
    }

    public async Task<List<Product>> GetPagedProductsAsync(int pageNumber, int pageSize)
    {
        int skip = (pageNumber - 1) * pageSize;

        return await _context.Products
            .Include(p => p.Supplier)
            .Include(p => p.Images)
            .OrderBy(p => p.CreatedAt)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();
    }
}
