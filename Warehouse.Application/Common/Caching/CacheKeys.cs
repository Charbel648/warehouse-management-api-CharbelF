namespace Warehouse.Application.Common.Caching;

public static class CacheKeys
{
    public static string Products(bool onlyAvailable)
    {
        return $"products:list:onlyAvailable:{onlyAvailable}";
    }

    public const string InventoryDashboard = "inventory:dashboard";
}
