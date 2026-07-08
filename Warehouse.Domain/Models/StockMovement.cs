namespace Warehouse.Domain.Models;

public class StockMovement
{
    public string Id { get; private set; } = Guid.NewGuid().ToString();

    public string ProductId { get; private set; }

    public int QuantityChanged { get; private set; }

    public StockMovementType Type { get; private set; }

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public StockMovement(
        string productId,
        int quantityChanged,
        StockMovementType type)
    {
        ProductId = productId;
        QuantityChanged = quantityChanged;
        Type = type;
    }
}

public enum StockMovementType
{
    StockIn,
    StockOut,
    Adjustment
}
