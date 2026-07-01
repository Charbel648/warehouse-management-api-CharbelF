using System.ComponentModel.DataAnnotations;

namespace warehouse_management.Contracts;

public class CreateProductRequest
{
    [Required] 
    [MaxLength(50)]
    public string Name { get; set; }
    public string? SKU { get; set; }
    public string? Description { get; set; }
    [Required] public double Price { get; set; }
    public int QuantityInStock { get; set; }
    public string? SupplierName { get; set; }
    public DateTime ExpiryDate { get; set; }
}