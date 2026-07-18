using System.ComponentModel.DataAnnotations;

namespace Warehouse.Presentation.Contracts;

public class StockAdjustmentRequest : IValidatableObject
{
    [Required(ErrorMessage = "Product id is required")]
    [RegularExpression(
        @"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
        ErrorMessage = "Product id must be a valid GUID")]
    public string ProductId { get; set; } = string.Empty;

    [Range(-100000, 100000, ErrorMessage = "Quantity change is outside allowed range")]
    public int QuantityChange { get; set; }

    [Required(ErrorMessage = "Reason is required")]
    [StringLength(300, MinimumLength = 3, ErrorMessage = "Reason must be between 3 and 300 characters")]
    public string Reason { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (QuantityChange == 0)
        {
            yield return new ValidationResult(
                "Quantity change cannot be zero",
                new[] { nameof(QuantityChange) });
        }
    }
}
