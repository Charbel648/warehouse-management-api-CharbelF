namespace Warehouse.Domain.Models;

public class WarehouseFile
{
    public string FileId { get; private set; } = Guid.NewGuid().ToString();

    public string RelatedEntityId { get; private set; } = string.Empty;

    public string RelatedEntityType { get; private set; } = string.Empty;

    public string FileCategory { get; private set; } = string.Empty;

    public string OriginalFileName { get; private set; } = string.Empty;

    public string ObjectKey { get; private set; } = string.Empty;

    public string ContentType { get; private set; } = string.Empty;

    public long SizeInBytes { get; private set; }

    public string UploadedByFirebaseUid { get; private set; } = string.Empty;

    public DateTime UploadedAt { get; private set; }

    private WarehouseFile()
    {
    }

    public WarehouseFile(
        string relatedEntityId,
        string relatedEntityType,
        string fileCategory,
        string originalFileName,
        string objectKey,
        string contentType,
        long sizeInBytes,
        string uploadedByFirebaseUid)
    {
        FileId = Guid.NewGuid().ToString();
        RelatedEntityId = relatedEntityId;
        RelatedEntityType = relatedEntityType;
        FileCategory = fileCategory;
        OriginalFileName = originalFileName;
        ObjectKey = objectKey;
        ContentType = contentType;
        SizeInBytes = sizeInBytes;
        UploadedByFirebaseUid = uploadedByFirebaseUid;
        UploadedAt = DateTime.UtcNow;
    }
}
