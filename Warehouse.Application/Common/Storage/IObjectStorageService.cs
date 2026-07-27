namespace Warehouse.Application.Common.Storage;

public interface IObjectStorageService
{
    Task<StoredObjectResult> UploadAsync(
        string objectKey,
        Stream content,
        string contentType,
        long sizeInBytes,
        CancellationToken cancellationToken);

    Task<StoredObjectDownloadResult> DownloadAsync(
        string objectKey,
        string fileName,
        string contentType,
        CancellationToken cancellationToken);
}

