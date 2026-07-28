using FluentAssertions;
using Moq;
using Warehouse.Api.UnitTests.Builders;
using Warehouse.Application.Common.Auth;
using Warehouse.Application.Common.Messaging;
using Warehouse.Application.Common.Storage;
using Warehouse.Application.Files.Commands.UploadProductImage;
using Warehouse.Application.Files.Commands.UploadSupplierDocument;
using Warehouse.Domain.Exceptions;
using Warehouse.Domain.Models;
using Warehouse.Domain.Repositories;

namespace Warehouse.Api.UnitTests.Files;

public class FileUploadHandlerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock = new();
    private readonly Mock<ISupplierRepository> _supplierRepositoryMock = new();
    private readonly Mock<IWarehouseFileRepository> _warehouseFileRepositoryMock = new();
    private readonly Mock<IObjectStorageService> _objectStorageServiceMock = new();
    private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();
    private readonly Mock<IWarehouseEventPublisher> _warehouseEventPublisherMock = new();

    public FileUploadHandlerTests()
    {
        _currentUserServiceMock
            .Setup(service => service.FirebaseUid)
            .Returns("firebase-user-123");

        _objectStorageServiceMock
            .Setup(service => service.UploadAsync(
                It.IsAny<string>(),
                It.IsAny<Stream>(),
                It.IsAny<string>(),
                It.IsAny<long>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StoredObjectResult
            {
                ObjectKey = "test-object-key"
            });

        _warehouseFileRepositoryMock
            .Setup(repository => repository.AddAsync(
                It.IsAny<WarehouseFile>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _warehouseEventPublisherMock
            .Setup(publisher => publisher.PublishAsync(
                It.IsAny<WarehouseNotificationEvent>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }

    [Fact]
    public async Task UploadProductImage_WithValidJpg_ShouldSucceed()
    {
        var product = new ProductBuilder().Build();

        _productRepositoryMock
            .Setup(repository => repository.GetByIdAsync(product.Id))
            .ReturnsAsync(product);

        var handler = CreateProductImageHandler();

        var command = CreateProductImageCommand(
            product.Id,
            "image.jpg",
            "image/jpeg",
            1024);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.RelatedEntityId.Should().Be(product.Id);
        result.RelatedEntityType.Should().Be("product");
        result.FileCategory.Should().Be("product-image");
        result.OriginalFileName.Should().Be("image.jpg");
        result.ContentType.Should().Be("image/jpeg");

        _objectStorageServiceMock.Verify(service => service.UploadAsync(
            It.Is<string>(key => key.StartsWith($"products/{product.Id}/images/") && key.EndsWith(".jpg")),
            It.IsAny<Stream>(),
            "image/jpeg",
            1024,
            It.IsAny<CancellationToken>()), Times.Once);

        _warehouseFileRepositoryMock.Verify(repository => repository.AddAsync(
            It.Is<WarehouseFile>(file =>
                file.RelatedEntityId == product.Id &&
                file.RelatedEntityType == "product" &&
                file.FileCategory == "product-image" &&
                file.OriginalFileName == "image.jpg" &&
                file.ObjectKey.StartsWith($"products/{product.Id}/images/") &&
                file.ObjectKey.EndsWith(".jpg")),
            It.IsAny<CancellationToken>()), Times.Once);

        _warehouseEventPublisherMock.Verify(publisher => publisher.PublishAsync(
            It.Is<WarehouseNotificationEvent>(notification =>
                notification.EventType == "WarehouseFileUploaded" &&
                notification.RelatedEntityId == product.Id &&
                notification.RelatedEntityType == "product" &&
                notification.FileCategory == "product-image" &&
                notification.FileName == "image.jpg"),
            "file.uploaded",
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UploadProductImage_WithValidPng_ShouldSucceed()
    {
        var product = new ProductBuilder().Build();

        _productRepositoryMock
            .Setup(repository => repository.GetByIdAsync(product.Id))
            .ReturnsAsync(product);

        var handler = CreateProductImageHandler();

        var command = CreateProductImageCommand(
            product.Id,
            "image.png",
            "image/png",
            1024);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.OriginalFileName.Should().Be("image.png");
        result.ContentType.Should().Be("image/png");

        _objectStorageServiceMock.Verify(service => service.UploadAsync(
            It.Is<string>(key => key.StartsWith($"products/{product.Id}/images/") && key.EndsWith(".png")),
            It.IsAny<Stream>(),
            "image/png",
            1024,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UploadProductImage_WithInvalidExtension_ShouldThrowException()
    {
        var product = new ProductBuilder().Build();

        _productRepositoryMock
            .Setup(repository => repository.GetByIdAsync(product.Id))
            .ReturnsAsync(product);

        var handler = CreateProductImageHandler();

        var command = CreateProductImageCommand(
            product.Id,
            "image.txt",
            "text/plain",
            1024);

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*JPG and PNG*");

        _objectStorageServiceMock.Verify(service => service.UploadAsync(
            It.IsAny<string>(),
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<long>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UploadProductImage_WithFileMoreThanTwoMb_ShouldThrowException()
    {
        var product = new ProductBuilder().Build();

        _productRepositoryMock
            .Setup(repository => repository.GetByIdAsync(product.Id))
            .ReturnsAsync(product);

        var handler = CreateProductImageHandler();

        var command = CreateProductImageCommand(
            product.Id,
            "large-image.jpg",
            "image/jpeg",
            (2 * 1024 * 1024) + 1);

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*2 MB*");

        _objectStorageServiceMock.Verify(service => service.UploadAsync(
            It.IsAny<string>(),
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<long>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UploadProductImage_ShouldGenerateCorrectUploadPath()
    {
        var product = new ProductBuilder().Build();

        _productRepositoryMock
            .Setup(repository => repository.GetByIdAsync(product.Id))
            .ReturnsAsync(product);

        var handler = CreateProductImageHandler();

        var command = CreateProductImageCommand(
            product.Id,
            "product-photo.jpeg",
            "image/jpeg",
            1024);

        await handler.Handle(command, CancellationToken.None);

        _objectStorageServiceMock.Verify(service => service.UploadAsync(
            It.Is<string>(key =>
                key.StartsWith($"products/{product.Id}/images/") &&
                key.EndsWith(".jpeg")),
            It.IsAny<Stream>(),
            "image/jpeg",
            1024,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UploadSupplierDocument_WithValidPdf_ShouldSucceed()
    {
        var supplier = new SupplierBuilder().Build();

        _supplierRepositoryMock
            .Setup(repository => repository.GetByIdAsync(supplier.Id))
            .ReturnsAsync(supplier);

        var handler = CreateSupplierDocumentHandler();

        var command = CreateSupplierDocumentCommand(
            supplier.Id,
            "document.pdf",
            "application/pdf",
            1024);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.RelatedEntityId.Should().Be(supplier.Id);
        result.RelatedEntityType.Should().Be("supplier");
        result.FileCategory.Should().Be("supplier-document");
        result.OriginalFileName.Should().Be("document.pdf");
        result.ContentType.Should().Be("application/pdf");

        _objectStorageServiceMock.Verify(service => service.UploadAsync(
            It.Is<string>(key => key.StartsWith($"suppliers/{supplier.Id}/documents/") && key.EndsWith(".pdf")),
            It.IsAny<Stream>(),
            "application/pdf",
            1024,
            It.IsAny<CancellationToken>()), Times.Once);

        _warehouseEventPublisherMock.Verify(publisher => publisher.PublishAsync(
            It.Is<WarehouseNotificationEvent>(notification =>
                notification.EventType == "WarehouseFileUploaded" &&
                notification.RelatedEntityId == supplier.Id &&
                notification.RelatedEntityType == "supplier" &&
                notification.FileCategory == "supplier-document" &&
                notification.FileName == "document.pdf"),
            "file.uploaded",
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UploadSupplierDocument_WithValidPng_ShouldSucceed()
    {
        var supplier = new SupplierBuilder().Build();

        _supplierRepositoryMock
            .Setup(repository => repository.GetByIdAsync(supplier.Id))
            .ReturnsAsync(supplier);

        var handler = CreateSupplierDocumentHandler();

        var command = CreateSupplierDocumentCommand(
            supplier.Id,
            "document.png",
            "image/png",
            1024);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.OriginalFileName.Should().Be("document.png");
        result.ContentType.Should().Be("image/png");

        _objectStorageServiceMock.Verify(service => service.UploadAsync(
            It.Is<string>(key => key.StartsWith($"suppliers/{supplier.Id}/documents/") && key.EndsWith(".png")),
            It.IsAny<Stream>(),
            "image/png",
            1024,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UploadSupplierDocument_WithInvalidExtension_ShouldThrowException()
    {
        var supplier = new SupplierBuilder().Build();

        _supplierRepositoryMock
            .Setup(repository => repository.GetByIdAsync(supplier.Id))
            .ReturnsAsync(supplier);

        var handler = CreateSupplierDocumentHandler();

        var command = CreateSupplierDocumentCommand(
            supplier.Id,
            "document.exe",
            "application/octet-stream",
            1024);

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*PDF, JPG, and PNG*");

        _objectStorageServiceMock.Verify(service => service.UploadAsync(
            It.IsAny<string>(),
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<long>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    private UploadProductImageCommandHandler CreateProductImageHandler()
    {
        return new UploadProductImageCommandHandler(
            _productRepositoryMock.Object,
            _warehouseFileRepositoryMock.Object,
            _objectStorageServiceMock.Object,
            _currentUserServiceMock.Object,
            _warehouseEventPublisherMock.Object);
    }

    private UploadSupplierDocumentCommandHandler CreateSupplierDocumentHandler()
    {
        return new UploadSupplierDocumentCommandHandler(
            _supplierRepositoryMock.Object,
            _warehouseFileRepositoryMock.Object,
            _objectStorageServiceMock.Object,
            _currentUserServiceMock.Object,
            _warehouseEventPublisherMock.Object);
    }

    private static UploadProductImageCommand CreateProductImageCommand(
        string productId,
        string fileName,
        string contentType,
        long sizeInBytes)
    {
        return new UploadProductImageCommand
        {
            ProductId = productId,
            Content = new MemoryStream(new byte[Math.Min((int)sizeInBytes, 1024)]),
            FileName = fileName,
            ContentType = contentType,
            SizeInBytes = sizeInBytes
        };
    }

    private static UploadSupplierDocumentCommand CreateSupplierDocumentCommand(
        string supplierId,
        string fileName,
        string contentType,
        long sizeInBytes)
    {
        return new UploadSupplierDocumentCommand
        {
            SupplierId = supplierId,
            Content = new MemoryStream(new byte[Math.Min((int)sizeInBytes, 1024)]),
            FileName = fileName,
            ContentType = contentType,
            SizeInBytes = sizeInBytes
        };
    }
}
