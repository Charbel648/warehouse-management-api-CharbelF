using System.ComponentModel.DataAnnotations;

namespace Warehouse.Presentation.Contracts;

public class CreateSupplierRequest
{
    [Required(ErrorMessage = "Supplier name is required")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Supplier name must be between 2 and 150 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Country is required")]
    [StringLength(100, ErrorMessage = "Country cannot exceed 100 characters")]
    public string Country { get; set; } = string.Empty;

    [Required(ErrorMessage = "Contact email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters")]
    public string ContactEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required")]
    [StringLength(50, ErrorMessage = "Phone number cannot exceed 50 characters")]
    [RegularExpression(@"^[0-9+\-\s()]+$", ErrorMessage = "Invalid phone number format")]
    public string PhoneNumber { get; set; } = string.Empty;
}

