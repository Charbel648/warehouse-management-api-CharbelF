namespace Warehouse.Domain.Models;

public class ProductImage
{
    public string ProductId { get; private set; }

    public string FileName { get; private set; }

    public string FilePath { get; private set; }

    public ProductImage(string productId, string fileName, string filePath)
    {
        ProductId = productId;
        FileName = fileName;
        FilePath = filePath;
    }
}
