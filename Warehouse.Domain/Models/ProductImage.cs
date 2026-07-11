namespace Warehouse.Domain.Models;

public class ProductImage
{
    public string ProductImageId { get; private set; } = Guid.NewGuid().ToString();

    public string ProductId { get; private set; } = string.Empty;

    public string FileName { get; private set; } = string.Empty;

    public string FilePath { get; private set; } = string.Empty;

    public DateTime UploadedAt { get; private set; }

    public Product? Product { get; private set; }

    private ProductImage()
    {
    }

    public ProductImage(string productId, string fileName, string filePath)
    {
        ProductImageId = Guid.NewGuid().ToString();
        ProductId = productId;
        FileName = fileName;
        FilePath = filePath;
        UploadedAt = DateTime.UtcNow;
    }
}
