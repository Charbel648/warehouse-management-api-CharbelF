namespace Warehouse.Application.Products.Commands.ArchiveProduct;

public class ArchiveProductResponse
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public bool IsArchived { get; set; }

    public DateTime LastUpdatedAt { get; set; }
}

