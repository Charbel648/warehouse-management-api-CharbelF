using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace warehouse_management.Contracts;

public class UploadProductImageRequest
{
    [Required]
    public IFormFile Image { get; set; } = null!;
}