using Warehouse.Application.Common.Caching;

namespace Warehouse.Application.Common;

public static class WarehouseCacheInvalidator
{
    public static async Task InvalidateProductCachesAsync(
        ICacheService cacheService,
        CancellationToken cancellationToken)
    {
        await cacheService.RemoveAsync(CacheKeys.Products(false), cancellationToken);
        await cacheService.RemoveAsync(CacheKeys.Products(true), cancellationToken);
        await cacheService.RemoveAsync(CacheKeys.InventoryDashboard, cancellationToken);
    }
}
