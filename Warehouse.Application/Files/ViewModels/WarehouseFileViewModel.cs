namespace Warehouse.Application.Files.ViewModels;

public class WarehouseFileViewModel
{
    public string FileId { get; set; } = string.Empty;

    public string RelatedEntityId { get; set; } = string.Empty;

    public string RelatedEntityType { get; set; } = string.Empty;

    public string FileCategory { get; set; } = string.Empty;

    public string OriginalFileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public long SizeInBytes { get; set; }

    public DateTime UploadedAt { get; set; }
}

