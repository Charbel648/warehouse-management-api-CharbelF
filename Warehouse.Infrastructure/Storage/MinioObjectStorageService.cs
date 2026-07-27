using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;
using Warehouse.Application.Common.Storage;
using Warehouse.Domain.Exceptions;

namespace Warehouse.Infrastructure.Storage;

public class MinioObjectStorageService : IObjectStorageService
{
    private readonly IMinioClient _minioClient;
    private readonly ILogger<MinioObjectStorageService> _logger;
    private readonly string _bucketName;

    public MinioObjectStorageService(
        IMinioClient minioClient,
        IConfiguration configuration,
        ILogger<MinioObjectStorageService> logger)
    {
        _minioClient = minioClient;
        _logger = logger;
        _bucketName = configuration["Minio:BucketName"] ?? "warehouse-assets";
    }

    public async Task<StoredObjectResult> UploadAsync(
        string objectKey,
        Stream content,
        string contentType,
        long sizeInBytes,
        CancellationToken cancellationToken)
    {
        try
        {
            await EnsureBucketExistsAsync(cancellationToken);

            if (content.CanSeek)
                content.Position = 0;

            await _minioClient.PutObjectAsync(
                new PutObjectArgs()
                    .WithBucket(_bucketName)
                    .WithObject(objectKey)
                    .WithStreamData(content)
                    .WithObjectSize(sizeInBytes)
                    .WithContentType(contentType),
                cancellationToken);

            _logger.LogInformation(
                "Uploaded warehouse file to MinIO. Bucket: {BucketName}, ObjectKey: {ObjectKey}, SizeInBytes: {SizeInBytes}",
                _bucketName,
                objectKey,
                sizeInBytes);

            return new StoredObjectResult
            {
                ObjectKey = objectKey,
                ContentType = contentType,
                SizeInBytes = sizeInBytes
            };
        }
        catch (BusinessRuleException)
        {
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Failed to upload warehouse file to MinIO. Bucket: {BucketName}, ObjectKey: {ObjectKey}",
                _bucketName,
                objectKey);

            throw new BusinessRuleException("Unable to upload file to object storage");
        }
    }

    public async Task<StoredObjectDownloadResult> DownloadAsync(
        string objectKey,
        string fileName,
        string contentType,
        CancellationToken cancellationToken)
    {
        try
        {
            var memoryStream = new MemoryStream();

            await _minioClient.GetObjectAsync(
                new GetObjectArgs()
                    .WithBucket(_bucketName)
                    .WithObject(objectKey)
                    .WithCallbackStream(stream =>
                    {
                        stream.CopyTo(memoryStream);
                    }),
                cancellationToken);

            memoryStream.Position = 0;

            return new StoredObjectDownloadResult
            {
                Content = memoryStream,
                ContentType = contentType,
                FileName = fileName
            };
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Failed to download warehouse file from MinIO. Bucket: {BucketName}, ObjectKey: {ObjectKey}",
                _bucketName,
                objectKey);

            throw new BusinessRuleException("Unable to download file from object storage");
        }
    }

    private async Task EnsureBucketExistsAsync(CancellationToken cancellationToken)
    {
        bool bucketExists = await _minioClient.BucketExistsAsync(
            new BucketExistsArgs().WithBucket(_bucketName),
            cancellationToken);

        if (bucketExists)
            return;

        await _minioClient.MakeBucketAsync(
            new MakeBucketArgs().WithBucket(_bucketName),
            cancellationToken);

        _logger.LogInformation(
            "Created MinIO bucket {BucketName}",
            _bucketName);
    }
}

