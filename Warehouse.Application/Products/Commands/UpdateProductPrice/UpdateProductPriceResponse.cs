namespace Warehouse.Application.Products.Commands.UpdateProductPrice;

public class UpdateProductPriceResponse
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public DateTime LastUpdatedAt { get; set; }
}

