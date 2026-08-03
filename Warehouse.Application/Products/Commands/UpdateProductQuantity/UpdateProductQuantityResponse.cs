namespace Warehouse.Application.Products.Commands.UpdateProductQuantity;

public class UpdateProductQuantityResponse
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public int QuantityInStock { get; set; }

    public DateTime LastUpdatedAt { get; set; }
}

