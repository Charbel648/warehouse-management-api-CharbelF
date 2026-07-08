using System.ComponentModel.DataAnnotations;

namespace Warehouse.Presentation.Contracts;

public class CreateProductRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string SKU { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int QuantityInStock { get; set; }

    public string SupplierName { get; set; } = string.Empty;

    public DateTime ExpiryDate { get; set; }
}