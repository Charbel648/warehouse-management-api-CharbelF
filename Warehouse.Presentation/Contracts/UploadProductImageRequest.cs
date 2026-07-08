using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Warehouse.Presentation.Contracts;

public class UploadProductImageRequest
{
    [Required]
    public IFormFile Image { get; set; } = null!;
}