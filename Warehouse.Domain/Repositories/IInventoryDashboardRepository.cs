using Warehouse.Domain.Models;

namespace Warehouse.Domain.Repositories;

public interface IInventoryDashboardRepository
{
    Task<List<DashboardGraphPoint>> GetProductStatusGraphAsync(CancellationToken cancellationToken);

    Task<List<DashboardGraphPoint>> GetStockLevelGraphAsync(CancellationToken cancellationToken);

    Task<List<DashboardGraphPoint>> GetSuppliersByCountryGraphAsync(CancellationToken cancellationToken);
}

