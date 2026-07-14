using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Warehouse.Infrastructure.Persistence;

namespace Warehouse.Presentation.Controllers;

[ApiController]
[Route("api/inventory")]
public class InventoryController : ControllerBase
{
    private readonly IDbContextFactory<WarehouseDbContext> _contextFactory;

    public InventoryController(IDbContextFactory<WarehouseDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    [HttpGet("dashboard")]
    public async Task<ActionResult> GetDashboard(CancellationToken cancellationToken)
    {
        Task<int> totalProductsTask = CountProductsAsync(cancellationToken);
        Task<int> totalSuppliersTask = CountSuppliersAsync(cancellationToken);
        Task<int> archivedProductsTask = CountArchivedProductsAsync(cancellationToken);
        Task<int> lowStockProductsTask = CountLowStockProductsAsync(cancellationToken);
        Task<int> totalStockQuantityTask = SumStockQuantityAsync(cancellationToken);

        await Task.WhenAll(
            totalProductsTask,
            totalSuppliersTask,
            archivedProductsTask,
            lowStockProductsTask,
            totalStockQuantityTask);

        return Ok(new
        {
            TotalProducts = await totalProductsTask,
            TotalSuppliers = await totalSuppliersTask,
            ArchivedProducts = await archivedProductsTask,
            LowStockProducts = await lowStockProductsTask,
            TotalStockQuantity = await totalStockQuantityTask
        });
    }

    private async Task<int> CountProductsAsync(CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        return await context.Products.CountAsync(cancellationToken);
    }

    private async Task<int> CountSuppliersAsync(CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        return await context.Suppliers.CountAsync(cancellationToken);
    }

    private async Task<int> CountArchivedProductsAsync(CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        return await context.Products.CountAsync(
            product => product.IsArchived,
            cancellationToken);
    }

    private async Task<int> CountLowStockProductsAsync(CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        return await context.Products.CountAsync(
            product => product.QuantityInStock < 10 && !product.IsArchived,
            cancellationToken);
    }

    private async Task<int> SumStockQuantityAsync(CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        return await context.Products.SumAsync(
            product => product.QuantityInStock,
            cancellationToken);
    }
}
