namespace Warehouse.Application.Products.Commands.AssignSupplierToProduct;

public class AssignSupplierToProductResponse
{
    public string ProductId { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public string SupplierId { get; set; } = string.Empty;

    public string SupplierName { get; set; } = string.Empty;

    public DateTime LastUpdatedAt { get; set; }
}
