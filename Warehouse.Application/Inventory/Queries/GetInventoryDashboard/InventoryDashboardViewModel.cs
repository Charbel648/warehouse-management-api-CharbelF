namespace Warehouse.Application.Inventory.Queries.GetInventoryDashboard;

public class InventoryDashboardViewModel
{
    public List<InventoryDashboardGraphViewModel> Graphs { get; set; } = new();
}

public class InventoryDashboardGraphViewModel
{
    public string Name { get; set; } = string.Empty;

    public bool IsAvailable { get; set; }

    public string? ErrorMessage { get; set; }

    public List<InventoryDashboardGraphPointViewModel> Points { get; set; } = new();
}

public class InventoryDashboardGraphPointViewModel
{
    public string Label { get; set; } = string.Empty;

    public int Value { get; set; }
}
