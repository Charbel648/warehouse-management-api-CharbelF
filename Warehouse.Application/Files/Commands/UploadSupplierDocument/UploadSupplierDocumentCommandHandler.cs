using MediatR;
using Warehouse.Application.Common.Auth;
using Warehouse.Application.Common.Storage;
using Warehouse.Application.Files.ViewModels;
using Warehouse.Domain.Exceptions;
using Warehouse.Domain.Models;
using Warehouse.Domain.Repositories;

namespace Warehouse.Application.Files.Commands.UploadSupplierDocument;

public class UploadSupplierDocumentCommandHandler
    : IRequestHandler<UploadSupplierDocumentCommand, WarehouseFileViewModel>
{
    private const long MaxDocumentSizeInBytes = 5 * 1024 * 1024;

    private static readonly HashSet<string> AllowedDocumentContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "application/pdf",
        "image/jpeg",
        "image/png"
    };

    private readonly ISupplierRepository _supplierRepository;
    private readonly IWarehouseFileRepository _warehouseFileRepository;
    private readonly IObjectStorageService _objectStorageService;
    private readonly ICurrentUserService _currentUserService;

    public UploadSupplierDocumentCommandHandler(
        ISupplierRepository supplierRepository,
        IWarehouseFileRepository warehouseFileRepository,
        IObjectStorageService objectStorageService,
        ICurrentUserService currentUserService)
    {
        _supplierRepository = supplierRepository;
        _warehouseFileRepository = warehouseFileRepository;
        _objectStorageService = objectStorageService;
        _currentUserService = currentUserService;
    }

    public async Task<WarehouseFileViewModel> Handle(
        UploadSupplierDocumentCommand request,
        CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.GetByIdAsync(request.SupplierId);

        if (supplier == null)
            throw new NotFoundException("Supplier", request.SupplierId);

        ValidateDocument(request);

        string extension = Path.GetExtension(request.FileName).ToLowerInvariant();
        string objectKey = $"suppliers/{request.SupplierId}/documents/{Guid.NewGuid():N}{extension}";

        await _objectStorageService.UploadAsync(
            objectKey,
            request.Content,
            request.ContentType,
            request.SizeInBytes,
            cancellationToken);

        var file = new WarehouseFile(
            request.SupplierId,
            "supplier",
            "supplier-document",
            request.FileName,
            objectKey,
            request.ContentType,
            request.SizeInBytes,
            _currentUserService.FirebaseUid);

        await _warehouseFileRepository.AddAsync(file, cancellationToken);

        return ToViewModel(file);
    }

    private static void ValidateDocument(UploadSupplierDocumentCommand request)
    {
        if (request.Content == Stream.Null || request.SizeInBytes <= 0)
            throw new BusinessRuleException("Document is required");

        if (request.SizeInBytes > MaxDocumentSizeInBytes)
            throw new BusinessRuleException("Document size cannot be more than 5 MB");

        if (!AllowedDocumentContentTypes.Contains(request.ContentType))
            throw new BusinessRuleException("Only PDF, JPG, and PNG documents are allowed");

        string extension = Path.GetExtension(request.FileName).ToLowerInvariant();

        if (extension != ".pdf" && extension != ".jpg" && extension != ".jpeg" && extension != ".png")
            throw new BusinessRuleException("Only PDF, JPG, and PNG documents are allowed");
    }

    private static WarehouseFileViewModel ToViewModel(WarehouseFile file)
    {
        return new WarehouseFileViewModel
        {
            FileId = file.FileId,
            RelatedEntityId = file.RelatedEntityId,
            RelatedEntityType = file.RelatedEntityType,
            FileCategory = file.FileCategory,
            OriginalFileName = file.OriginalFileName,
            ContentType = file.ContentType,
            SizeInBytes = file.SizeInBytes,
            UploadedAt = file.UploadedAt
        };
    }
}
