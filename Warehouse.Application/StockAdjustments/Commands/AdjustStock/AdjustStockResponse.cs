namespace Warehouse.Application.StockAdjustments.Commands.AdjustStock;

public class AdjustStockResponse
{
    public string ProductId { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public int PreviousQuantity { get; set; }

    public int QuantityChange { get; set; }

    public int NewQuantity { get; set; }

    public string Reason { get; set; } = string.Empty;

    public DateTime UpdatedAt { get; set; }
}
