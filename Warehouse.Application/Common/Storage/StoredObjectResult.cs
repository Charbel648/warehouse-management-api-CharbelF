namespace Warehouse.Application.Common.Storage;

public class StoredObjectResult
{
    public string ObjectKey { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public long SizeInBytes { get; set; }
}

