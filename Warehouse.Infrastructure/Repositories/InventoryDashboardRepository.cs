using Microsoft.EntityFrameworkCore;
using Warehouse.Domain.Models;
using Warehouse.Domain.Repositories;
using Warehouse.Infrastructure.Persistence;

namespace Warehouse.Infrastructure.Repositories;

public class InventoryDashboardRepository : IInventoryDashboardRepository
{
    private readonly WarehouseDbContext _context;

    public InventoryDashboardRepository(WarehouseDbContext context)
    {
        _context = context;
    }

    public async Task<List<DashboardGraphPoint>> GetProductStatusGraphAsync(
        CancellationToken cancellationToken)
    {
        int activeProducts = await _context.Products
            .CountAsync(product => !product.IsArchived, cancellationToken);

        int archivedProducts = await _context.Products
            .CountAsync(product => product.IsArchived, cancellationToken);

        return new List<DashboardGraphPoint>
        {
            new DashboardGraphPoint
            {
                Label = "Active products",
                Value = activeProducts
            },
            new DashboardGraphPoint
            {
                Label = "Archived products",
                Value = archivedProducts
            }
        };
    }

    public async Task<List<DashboardGraphPoint>> GetStockLevelGraphAsync(
        CancellationToken cancellationToken)
    {
        int lowStockProducts = await _context.Products
            .CountAsync(product => product.QuantityInStock < 10 && !product.IsArchived, cancellationToken);

        int normalStockProducts = await _context.Products
            .CountAsync(product => product.QuantityInStock >= 10 && !product.IsArchived, cancellationToken);

        return new List<DashboardGraphPoint>
        {
            new DashboardGraphPoint
            {
                Label = "Low stock products",
                Value = lowStockProducts
            },
            new DashboardGraphPoint
            {
                Label = "Normal stock products",
                Value = normalStockProducts
            }
        };
    }

    public async Task<List<DashboardGraphPoint>> GetSuppliersByCountryGraphAsync(
        CancellationToken cancellationToken)
    {
        return await _context.Suppliers
            .GroupBy(supplier => supplier.Country)
            .Select(group => new DashboardGraphPoint
            {
                Label = group.Key,
                Value = group.Count()
            })
            .ToListAsync(cancellationToken);
    }
}

