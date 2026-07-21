namespace Warehouse.Application.Common.Caching;

public static partial class CacheKeys
{
    private const string Prefix = "warehouse-management-api";

    public static string ProductListByAvailability(bool onlyAvailable)
    {
        return $"{Prefix}:products:list:filter:only-available:{onlyAvailable}";
    }

    public const string InventoryDashboardGraphs = "warehouse-management-api:inventory-dashboard:graphs:v1";
}
