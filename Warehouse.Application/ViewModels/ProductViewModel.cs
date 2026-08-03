namespace Warehouse.Application.ViewModels;

public class ProductViewModel
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string SKU { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int QuantityInStock { get; set; }

    public string SupplierId { get; set; } = string.Empty;

    public string SupplierName { get; set; } = string.Empty;

    public DateTime ExpiryDate { get; set; }

    public bool IsArchived { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastUpdatedAt { get; set; }

    public List<string> ImageUrls { get; set; } = new();
}

