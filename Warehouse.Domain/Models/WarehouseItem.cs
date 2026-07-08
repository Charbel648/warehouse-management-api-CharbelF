namespace Warehouse.Domain.Models;

public class WarehouseItem
{
    public string Id { get; private set; } = Guid.NewGuid().ToString();

    public string ProductId { get; private set; }

    public string LocationCode { get; private set; }

    public int Quantity { get; private set; }

    public WarehouseItem(
        string productId,
        string locationCode,
        int quantity)
    {
        if (quantity < 0)
        {
            throw new ArgumentException("Quantity cannot be negative");
        }

        ProductId = productId;
        LocationCode = locationCode;
        Quantity = quantity;
    }
}