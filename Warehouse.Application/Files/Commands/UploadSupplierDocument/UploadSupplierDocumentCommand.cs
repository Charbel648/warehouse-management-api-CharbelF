using MediatR;
using Warehouse.Application.Files.ViewModels;

namespace Warehouse.Application.Files.Commands.UploadSupplierDocument;

public class UploadSupplierDocumentCommand : IRequest<WarehouseFileViewModel>
{
    public string SupplierId { get; set; } = string.Empty;

    public Stream Content { get; set; } = Stream.Null;

    public string FileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public long SizeInBytes { get; set; }
}
