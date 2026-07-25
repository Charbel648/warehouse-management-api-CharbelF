using System.ComponentModel.DataAnnotations;

namespace Warehouse.Presentation.Contracts;

public class UploadSupplierDocumentRequest
{
    [Required(ErrorMessage = "Document is required")]
    public IFormFile Document { get; set; } = default!;
}
