using FluentAssertions;
using Moq;
using Warehouse.Api.UnitTests.Builders;
using Warehouse.Application.Products.Commands.AssignSupplierToProduct;
using Warehouse.Domain.Models;
using Warehouse.Domain.Repositories;

namespace Warehouse.Api.UnitTests.Products;

public class AssignSupplierArchivedProductTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock = new();
    private readonly Mock<ISupplierRepository> _supplierRepositoryMock = new();

    [Fact]
    public async Task AssignSupplierToProduct_WhenProductIsArchived_ShouldThrowAndNotUpdateRepository()
    {
        var product = new ProductBuilder()
            .WithName("Archived Product")
            .Build();

        product.Archive();

        var supplier = new Supplier(
            "Active Supplier",
            "Lebanon",
            "active.supplier@test.com",
            "+96100000000");

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

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Archived products cannot be updated*");

        product.SupplierId.Should().BeNull();
        product.SupplierName.Should().Be("Default Supplier");

        _productRepositoryMock.Verify(
            repository => repository.UpdateAsync(It.IsAny<Product>()),
            Times.Never);
    }
}
