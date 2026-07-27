using MediatR;
using Warehouse.Application.Common.Storage;

namespace Warehouse.Application.Files.Queries.DownloadWarehouseFile;

public class DownloadWarehouseFileQuery : IRequest<StoredObjectDownloadResult>
{
    public string FileId { get; set; } = string.Empty;
}

