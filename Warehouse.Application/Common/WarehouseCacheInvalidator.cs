using Warehouse.Application.Common.Caching;

namespace Warehouse.Application.Common;

public static class WarehouseCacheInvalidator
{
    public static async Task InvalidateProductCachesAsync(
        ICacheService cacheService,
        CancellationToken cancellationToken)
    {
        await cacheService.RemoveAsync(CacheKeys.ProductListByAvailability(false), cancellationToken);
        await cacheService.RemoveAsync(CacheKeys.ProductListByAvailability(true), cancellationToken);
        await cacheService.RemoveAsync(CacheKeys.InventoryDashboardGraphs, cancellationToken);
    }
}

