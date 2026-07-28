using Warehouse.Application.Common.Storage;

namespace Warehouse.Api.IntegrationTests.Infrastructure;

public class TestObjectStorageService : IObjectStorageService
{
    public Task<StoredObjectResult> UploadAsync(
        string objectKey,
        Stream content,
        string contentType,
        long sizeInBytes,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(default(StoredObjectResult)!);
    }

    public Task<StoredObjectDownloadResult> DownloadAsync(
        string objectKey,
        string fileName,
        string contentType,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(default(StoredObjectDownloadResult)!);
    }
}
