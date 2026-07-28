using FluentAssertions;
using Moq;
using Warehouse.Api.UnitTests.Builders;
using Warehouse.Application.Products.Commands.AssignSupplierToProduct;
using Warehouse.Application.Suppliers.Commands.CreateSupplier;
using Warehouse.Application.Suppliers.Commands.DeactivateSupplier;
using Warehouse.Domain.Models;
using Warehouse.Domain.Repositories;

namespace Warehouse.Api.UnitTests.Suppliers;

public class SupplierHandlerTests
{
    private readonly Mock<ISupplierRepository> _supplierRepositoryMock = new();
    private readonly Mock<IProductRepository> _productRepositoryMock = new();

    [Fact]
    public async Task CreateSupplier_WithValidSupplier_ShouldSucceed()
    {
        Supplier? capturedSupplier = null;

        _supplierRepositoryMock
            .Setup(repository => repository.EmailExistsAsync("supplier@test.com"))
            .ReturnsAsync(false);

        _supplierRepositoryMock
            .Setup(repository => repository.AddAsync(It.IsAny<Supplier>()))
            .Callback<Supplier>(supplier => capturedSupplier = supplier)
            .Returns(Task.CompletedTask);

        var handler = new CreateSupplierHandler(_supplierRepositoryMock.Object);

        var command = new CreateSupplierCommand
        {
            Name = "Default Supplier",
            Country = "Lebanon",
            ContactEmail = "supplier@test.com",
            PhoneNumber = "+96100000000"
        };

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().NotBeNullOrWhiteSpace();
        result.Name.Should().Be("Default Supplier");
        result.Country.Should().Be("Lebanon");
        result.ContactEmail.Should().Be("supplier@test.com");
        result.IsActive.Should().BeTrue();

        capturedSupplier.Should().NotBeNull();
        _supplierRepositoryMock.Verify(repository => repository.AddAsync(It.IsAny<Supplier>()), Times.Once);
    }

    [Fact]
    public async Task CreateSupplier_WithDuplicateEmail_ShouldThrowException()
    {
        _supplierRepositoryMock
            .Setup(repository => repository.EmailExistsAsync("supplier@test.com"))
            .ReturnsAsync(true);

        var handler = new CreateSupplierHandler(_supplierRepositoryMock.Object);

        var command = new CreateSupplierCommand
        {
            Name = "Default Supplier",
            Country = "Lebanon",
            ContactEmail = "supplier@test.com",
            PhoneNumber = "+96100000000"
        };

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*same email*");

        _supplierRepositoryMock.Verify(repository => repository.AddAsync(It.IsAny<Supplier>()), Times.Never);
    }

    [Fact]
    public async Task DeactivateSupplier_WithExistingSupplier_ShouldDeactivateSupplier()
    {
        var supplier = new SupplierBuilder().Build();

        _supplierRepositoryMock
            .Setup(repository => repository.GetByIdAsync(supplier.Id))
            .ReturnsAsync(supplier);

        var handler = new DeactivateSupplierHandler(_supplierRepositoryMock.Object);

        var result = await handler.Handle(
            new DeactivateSupplierCommand
            {
                SupplierId = supplier.Id
            },
            CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(supplier.Id);
        result.IsActive.Should().BeFalse();
        supplier.IsActive.Should().BeFalse();

        _supplierRepositoryMock.Verify(repository => repository.UpdateAsync(supplier), Times.Once);
    }

    [Fact]
    public async Task DeactivateSupplier_WithMissingSupplier_ShouldReturnNull()
    {
        _supplierRepositoryMock
            .Setup(repository => repository.GetByIdAsync("missing-supplier-id"))
            .ReturnsAsync((Supplier?)null);

        var handler = new DeactivateSupplierHandler(_supplierRepositoryMock.Object);

        var result = await handler.Handle(
            new DeactivateSupplierCommand
            {
                SupplierId = "missing-supplier-id"
            },
            CancellationToken.None);

        result.Should().BeNull();

        _supplierRepositoryMock.Verify(repository => repository.UpdateAsync(It.IsAny<Supplier>()), Times.Never);
    }

    [Fact]
    public async Task AssignSupplierToProduct_WithValidProductAndSupplier_ShouldAssignSupplier()
    {
        var product = new ProductBuilder()
            .WithName("Laptop")
            .Build();

        var supplier = new SupplierBuilder()
            .WithName("Tech Supplier")
            .Build();

        _productRepositoryMock
            .Setup(repository => repository.GetByIdAsync(product.Id))
            .ReturnsAsync(product);

        _supplierRepositoryMock
            .Setup(repository => repository.GetByIdAsync(supplier.Id))
            .ReturnsAsync(supplier);

        var handler = new AssignSupplierToProductHandler(
            _productRepositoryMock.Object,
            _supplierRepositoryMock.Object);

        var result = await handler.Handle(
            new AssignSupplierToProductCommand
            {
                ProductId = product.Id,
                SupplierId = supplier.Id
            },
            CancellationToken.None);

        result.Should().NotBeNull();
        result!.ProductId.Should().Be(product.Id);
        result.ProductName.Should().Be(product.Name);
        result.SupplierId.Should().Be(supplier.Id);
        result.SupplierName.Should().Be(supplier.Name);

        _productRepositoryMock.Verify(repository => repository.UpdateAsync(product), Times.Once);
    }

    [Fact]
    public async Task AssignSupplierToProduct_WithArchivedProduct_ShouldThrowException()
    {
        var product = new ProductBuilder()
            .WithName("Laptop")
            .Build();

        product.Archive();

        var supplier = new SupplierBuilder()
            .WithName("Tech Supplier")
            .Build();

        _productRepositoryMock
            .Setup(repository => repository.GetByIdAsync(product.Id))
            .ReturnsAsync(product);

        _supplierRepositoryMock
            .Setup(repository => repository.GetByIdAsync(supplier.Id))
            .ReturnsAsync(supplier);

        var handler = new AssignSupplierToProductHandler(
            _productRepositoryMock.Object,
            _supplierRepositoryMock.Object);

        Func<Task> act = async () => await handler.Handle(
            new AssignSupplierToProductCommand
            {
                ProductId = product.Id,
                SupplierId = supplier.Id
            },
            CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();

        _productRepositoryMock.Verify(repository => repository.UpdateAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task AssignSupplierToProduct_WithMissingSupplier_ShouldThrowException()
    {
        var product = new ProductBuilder().Build();

        _productRepositoryMock
            .Setup(repository => repository.GetByIdAsync(product.Id))
            .ReturnsAsync(product);

        _supplierRepositoryMock
            .Setup(repository => repository.GetByIdAsync("missing-supplier-id"))
            .ReturnsAsync((Supplier?)null);

        var handler = new AssignSupplierToProductHandler(
            _productRepositoryMock.Object,
            _supplierRepositoryMock.Object);

        Func<Task> act = async () => await handler.Handle(
            new AssignSupplierToProductCommand
            {
                ProductId = product.Id,
                SupplierId = "missing-supplier-id"
            },
            CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Supplier not found*");

        _productRepositoryMock.Verify(repository => repository.UpdateAsync(It.IsAny<Product>()), Times.Never);
    }
}
