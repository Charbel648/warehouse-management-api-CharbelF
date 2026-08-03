namespace Warehouse.Application.Common.Storage;

public class StoredObjectDownloadResult
{
    public Stream Content { get; set; } = Stream.Null;

    public string ContentType { get; set; } = "application/octet-stream";

    public string FileName { get; set; } = string.Empty;
}

