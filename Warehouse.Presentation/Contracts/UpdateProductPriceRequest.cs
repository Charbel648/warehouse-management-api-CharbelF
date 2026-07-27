using System.ComponentModel.DataAnnotations;

namespace Warehouse.Presentation.Contracts;

public class UpdateProductPriceRequest
{
    [Range(typeof(decimal), "0.01", "999999999", ErrorMessage = "Price must be greater than zero")]
    public decimal Price { get; set; }
}

