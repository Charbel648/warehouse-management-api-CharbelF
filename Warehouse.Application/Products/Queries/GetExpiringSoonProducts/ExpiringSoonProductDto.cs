namespace Warehouse.Application.Products.Queries.GetExpiringSoonProducts;

public class ExpiringSoonProductDto
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string SKU { get; set; } = string.Empty;

    public int QuantityInStock { get; set; }

    public string SupplierName { get; set; } = string.Empty;

    public DateTime ExpiryDate { get; set; }

    public int DaysUntilExpiry { get; set; }
}
