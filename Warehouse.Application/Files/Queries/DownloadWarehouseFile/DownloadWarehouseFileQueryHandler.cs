using MediatR;
using Warehouse.Application.Common.Storage;
using Warehouse.Domain.Exceptions;
using Warehouse.Domain.Repositories;

namespace Warehouse.Application.Files.Queries.DownloadWarehouseFile;

public class DownloadWarehouseFileQueryHandler
    : IRequestHandler<DownloadWarehouseFileQuery, StoredObjectDownloadResult>
{
    private readonly IWarehouseFileRepository _warehouseFileRepository;
    private readonly IObjectStorageService _objectStorageService;

    public DownloadWarehouseFileQueryHandler(
        IWarehouseFileRepository warehouseFileRepository,
        IObjectStorageService objectStorageService)
    {
        _warehouseFileRepository = warehouseFileRepository;
        _objectStorageService = objectStorageService;
    }

    public async Task<StoredObjectDownloadResult> Handle(
        DownloadWarehouseFileQuery request,
        CancellationToken cancellationToken)
    {
        var file = await _warehouseFileRepository.GetByIdAsync(
            request.FileId,
            cancellationToken);

        if (file == null)
            throw new NotFoundException("WarehouseFile", request.FileId);

        return await _objectStorageService.DownloadAsync(
            file.ObjectKey,
            file.OriginalFileName,
            file.ContentType,
            cancellationToken);
    }
}
