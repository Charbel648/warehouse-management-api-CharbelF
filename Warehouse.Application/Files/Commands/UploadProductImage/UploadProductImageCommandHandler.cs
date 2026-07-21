using MediatR;
using Warehouse.Application.Common.Auth;
using Warehouse.Application.Common.Storage;
using Warehouse.Application.Files.ViewModels;
using Warehouse.Domain.Exceptions;
using Warehouse.Domain.Models;
using Warehouse.Domain.Repositories;

namespace Warehouse.Application.Files.Commands.UploadProductImage;

public class UploadProductImageCommandHandler
    : IRequestHandler<UploadProductImageCommand, WarehouseFileViewModel>
{
    private const long MaxImageSizeInBytes = 2 * 1024 * 1024;

    private static readonly HashSet<string> AllowedImageContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png"
    };

    private readonly IProductRepository _productRepository;
    private readonly IWarehouseFileRepository _warehouseFileRepository;
    private readonly IObjectStorageService _objectStorageService;
    private readonly ICurrentUserService _currentUserService;

    public UploadProductImageCommandHandler(
        IProductRepository productRepository,
        IWarehouseFileRepository warehouseFileRepository,
        IObjectStorageService objectStorageService,
        ICurrentUserService currentUserService)
    {
        _productRepository = productRepository;
        _warehouseFileRepository = warehouseFileRepository;
        _objectStorageService = objectStorageService;
        _currentUserService = currentUserService;
    }

    public async Task<WarehouseFileViewModel> Handle(
        UploadProductImageCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId);

        if (product == null)
            throw new NotFoundException("Product", request.ProductId);

        ValidateImage(request);

        string extension = Path.GetExtension(request.FileName).ToLowerInvariant();
        string objectKey = $"products/{request.ProductId}/images/{Guid.NewGuid():N}{extension}";

        await _objectStorageService.UploadAsync(
            objectKey,
            request.Content,
            request.ContentType,
            request.SizeInBytes,
            cancellationToken);

        var file = new WarehouseFile(
            request.ProductId,
            "product",
            "product-image",
            request.FileName,
            objectKey,
            request.ContentType,
            request.SizeInBytes,
            _currentUserService.FirebaseUid);

        await _warehouseFileRepository.AddAsync(file, cancellationToken);

        return ToViewModel(file);
    }

    private static void ValidateImage(UploadProductImageCommand request)
    {
        if (request.Content == Stream.Null || request.SizeInBytes <= 0)
            throw new BusinessRuleException("Image is required");

        if (request.SizeInBytes > MaxImageSizeInBytes)
            throw new BusinessRuleException("Image size cannot be more than 2 MB");

        if (!AllowedImageContentTypes.Contains(request.ContentType))
            throw new BusinessRuleException("Only JPG and PNG images are allowed");

        string extension = Path.GetExtension(request.FileName).ToLowerInvariant();

        if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
            throw new BusinessRuleException("Only JPG and PNG images are allowed");
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
