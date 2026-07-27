using System.ComponentModel.DataAnnotations;

namespace Warehouse.Presentation.Contracts;

public class CreateProductRequest : IValidatableObject
{
    [Required(ErrorMessage = "Product name is required")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Product name must be between 2 and 150 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "SKU is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "SKU must be between 2 and 100 characters")]
    [RegularExpression("^[A-Za-z0-9-]+$", ErrorMessage = "SKU can only contain letters, numbers, and dashes")]
    public string SKU { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string Description { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "999999999", ErrorMessage = "Price must be greater than zero")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative")]
    public int QuantityInStock { get; set; }

    [StringLength(150, ErrorMessage = "Supplier name cannot exceed 150 characters")]
    public string SupplierName { get; set; } = string.Empty;

    public DateTime ExpiryDate { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (QuantityInStock > 0 && ExpiryDate <= DateTime.UtcNow)
        {
            yield return new ValidationResult(
                "Expiry date must be in the future when product has stock",
                new[] { nameof(ExpiryDate), nameof(QuantityInStock) });
        }
    }
}

