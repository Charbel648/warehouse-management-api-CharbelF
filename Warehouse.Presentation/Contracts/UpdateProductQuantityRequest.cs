using System.ComponentModel.DataAnnotations;

namespace Warehouse.Presentation.Contracts;

public class UpdateProductQuantityRequest
{
    [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative")]
    public int QuantityInStock { get; set; }
}

