using System.ComponentModel.DataAnnotations;

namespace Warehouse.Presentation.Contracts;

public class UploadProductImageRequest
{
    [Required(ErrorMessage = "Image is required")]
    public IFormFile Image { get; set; } = null!;
}

