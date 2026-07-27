using AutoMapper;
using FluentAssertions;
using Moq;
using Warehouse.Api.UnitTests.Builders;
using Warehouse.Application.Products.Commands.ArchiveProduct;
using Warehouse.Application.Products.Commands.CreateProduct;
using Warehouse.Application.Products.Commands.UpdateProductPrice;
using Warehouse.Application.Products.Commands.UpdateProductQuantity;
using Warehouse.Application.Products.Queries.SearchProducts;
using Warehouse.Application.ViewModels;
using Warehouse.Domain.Models;
using Warehouse.Domain.Repositories;

namespace Warehouse.Api.UnitTests.Products;

public class ProductHandlerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock = new();

    [Fact]
    public async Task CreateProduct_WithValidProduct_ShouldSucceed()
    {
        Product? capturedProduct = null;

        _productRepositoryMock
            .Setup(repository => repository.SkuExistsAsync("SKU-001"))
            .ReturnsAsync(false);

        _productRepositoryMock
            .Setup(repository => repository.AddAsync(It.IsAny<Product>()))
            .Callback<Product>(product => capturedProduct = product)
            .Returns(Task.CompletedTask);

        var handler = new CreateProductHandler(_productRepositoryMock.Object);

        var command = new CreateProductCommand
        {
            Name = "Laptop",
            SKU = "SKU-001",
            Description = "Gaming laptop",
            Price = 1200,
            QuantityInStock = 5,
            SupplierName = "Tech Supplier",
            ExpiryDate = DateTime.UtcNow.AddYears(1)
        };

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().NotBeNullOrWhiteSpace();
        result.Name.Should().Be("Laptop");
        result.SKU.Should().Be("SKU-001");
        capturedProduct.Should().NotBeNull();
        _productRepositoryMock.Verify(repository => repository.AddAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task CreateProduct_WithDuplicateSku_ShouldThrowException()
    {
        _productRepositoryMock
            .Setup(repository => repository.SkuExistsAsync("SKU-001"))
            .ReturnsAsync(true);

        var handler = new CreateProductHandler(_productRepositoryMock.Object);

        var command = new CreateProductCommand
        {
            Name = "Laptop",
            SKU = "SKU-001",
            Description = "Gaming laptop",
            Price = 1200,
            QuantityInStock = 5,
            SupplierName = "Tech Supplier",
            ExpiryDate = DateTime.UtcNow.AddYears(1)
        };

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*same SKU*");

        _productRepositoryMock.Verify(repository => repository.AddAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task CreateProduct_ShouldAssignCreatedDate()
    {
        _productRepositoryMock
            .Setup(repository => repository.SkuExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        var handler = new CreateProductHandler(_productRepositoryMock.Object);

        var command = new CreateProductCommand
        {
            Name = "Mouse",
            SKU = "SKU-002",
            Description = "Wireless mouse",
            Price = 20,
            QuantityInStock = 15,
            SupplierName = "Tech Supplier",
            ExpiryDate = DateTime.UtcNow.AddYears(1)
        };

        var result = await handler.Handle(command, CancellationToken.None);

        result.CreatedAt.Should().NotBe(default);
    }

    [Fact]
    public async Task CreateProduct_ShouldGenerateId()
    {
        _productRepositoryMock
            .Setup(repository => repository.SkuExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        var handler = new CreateProductHandler(_productRepositoryMock.Object);

        var command = new CreateProductCommand
        {
            Name = "Keyboard",
            SKU = "SKU-003",
            Description = "Mechanical keyboard",
            Price = 80,
            QuantityInStock = 7,
            SupplierName = "Tech Supplier",
            ExpiryDate = DateTime.UtcNow.AddYears(1)
        };

        var result = await handler.Handle(command, CancellationToken.None);

        result.Id.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task SearchProducts_ByName_ShouldReturnMatches()
    {
        var products = new List<Product>
        {
            new ProductBuilder().WithName("Laptop").Build()
        };

        _productRepositoryMock
            .Setup(repository => repository.SearchAsync("Laptop", null))
            .ReturnsAsync(products);

        var mapperMock = new Mock<IMapper>();
        mapperMock
            .Setup(mapper => mapper.Map<List<ProductViewModel>>(products))
            .Returns(products.Select(product => new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                SKU = product.SKU,
                SupplierName = product.SupplierName
            }).ToList());

        var handler = new SearchProductsHandler(_productRepositoryMock.Object, mapperMock.Object);

        var result = await handler.Handle(new SearchProductsQuery { Name = "Laptop" }, CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].Name.Should().Be("Laptop");
    }

    [Fact]
    public async Task SearchProducts_BySupplier_ShouldReturnMatches()
    {
        var products = new List<Product>
        {
            new ProductBuilder().WithSupplierName("Apple").Build()
        };

        _productRepositoryMock
            .Setup(repository => repository.SearchAsync(null, "Apple"))
            .ReturnsAsync(products);

        var mapperMock = new Mock<IMapper>();
        mapperMock
            .Setup(mapper => mapper.Map<List<ProductViewModel>>(products))
            .Returns(products.Select(product => new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                SKU = product.SKU,
                SupplierName = product.SupplierName
            }).ToList());

        var handler = new SearchProductsHandler(_productRepositoryMock.Object, mapperMock.Object);

        var result = await handler.Handle(new SearchProductsQuery { Supplier = "Apple" }, CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].SupplierName.Should().Be("Apple");
    }

    [Fact]
    public async Task SearchProducts_ByNameAndSupplier_ShouldReturnIntersection()
    {
        var products = new List<Product>
        {
            new ProductBuilder().WithName("Laptop").WithSupplierName("Apple").Build()
        };

        _productRepositoryMock
            .Setup(repository => repository.SearchAsync("Laptop", "Apple"))
            .ReturnsAsync(products);

        var mapperMock = new Mock<IMapper>();
        mapperMock
            .Setup(mapper => mapper.Map<List<ProductViewModel>>(products))
            .Returns(products.Select(product => new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                SKU = product.SKU,
                SupplierName = product.SupplierName
            }).ToList());

        var handler = new SearchProductsHandler(_productRepositoryMock.Object, mapperMock.Object);

        var result = await handler.Handle(
            new SearchProductsQuery { Name = "Laptop", Supplier = "Apple" },
            CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].Name.Should().Be("Laptop");
        result[0].SupplierName.Should().Be("Apple");
    }

    [Fact]
    public async Task SearchProducts_WithEmptyFilters_ShouldThrowException()
    {
        var mapperMock = new Mock<IMapper>();
        var handler = new SearchProductsHandler(_productRepositoryMock.Object, mapperMock.Object);

        Func<Task> act = async () => await handler.Handle(new SearchProductsQuery(), CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*name or supplier*");
    }

    [Fact]
    public async Task UpdateQuantity_WithValidQuantity_ShouldUpdateStock()
    {
        var product = new ProductBuilder().WithQuantity(10).Build();

        _productRepositoryMock
            .Setup(repository => repository.GetByIdAsync(product.Id))
            .ReturnsAsync(product);

        var handler = new UpdateProductQuantityHandler(_productRepositoryMock.Object);

        var result = await handler.Handle(
            new UpdateProductQuantityCommand
            {
                ProductId = product.Id,
                QuantityInStock = 3
            },
            CancellationToken.None);

        result.Should().NotBeNull();
        result!.QuantityInStock.Should().Be(3);
        _productRepositoryMock.Verify(repository => repository.UpdateAsync(product), Times.Once);
    }

    [Fact]
    public async Task UpdateQuantity_WithNegativeQuantity_ShouldThrowException()
    {
        var product = new ProductBuilder().WithQuantity(10).Build();

        _productRepositoryMock
            .Setup(repository => repository.GetByIdAsync(product.Id))
            .ReturnsAsync(product);

        var handler = new UpdateProductQuantityHandler(_productRepositoryMock.Object);

        Func<Task> act = async () => await handler.Handle(
            new UpdateProductQuantityCommand
            {
                ProductId = product.Id,
                QuantityInStock = -1
            },
            CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdateQuantity_ShouldChangeLastUpdatedDate()
    {
        var product = new ProductBuilder().WithQuantity(10).Build();
        var oldDate = product.LastUpdatedAt;

        await Task.Delay(5);

        _productRepositoryMock
            .Setup(repository => repository.GetByIdAsync(product.Id))
            .ReturnsAsync(product);

        var handler = new UpdateProductQuantityHandler(_productRepositoryMock.Object);

        var result = await handler.Handle(
            new UpdateProductQuantityCommand
            {
                ProductId = product.Id,
                QuantityInStock = 4
            },
            CancellationToken.None);

        result!.LastUpdatedAt.Should().BeAfter(oldDate);
    }

    [Fact]
    public async Task UpdatePrice_WithValidPrice_ShouldUpdatePrice()
    {
        var product = new ProductBuilder().WithPrice(100).Build();

        _productRepositoryMock
            .Setup(repository => repository.GetByIdAsync(product.Id))
            .ReturnsAsync(product);

        var handler = new UpdateProductPriceHandler(_productRepositoryMock.Object);

        var result = await handler.Handle(
            new UpdateProductPriceCommand
            {
                ProductId = product.Id,
                Price = 250
            },
            CancellationToken.None);

        result.Should().NotBeNull();
        result!.Price.Should().Be(250);
        _productRepositoryMock.Verify(repository => repository.UpdateAsync(product), Times.Once);
    }

    [Fact]
    public async Task UpdatePrice_WithInvalidPrice_ShouldThrowException()
    {
        var product = new ProductBuilder().WithPrice(100).Build();

        _productRepositoryMock
            .Setup(repository => repository.GetByIdAsync(product.Id))
            .ReturnsAsync(product);

        var handler = new UpdateProductPriceHandler(_productRepositoryMock.Object);

        Func<Task> act = async () => await handler.Handle(
            new UpdateProductPriceCommand
            {
                ProductId = product.Id,
                Price = -5
            },
            CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ArchiveProduct_ShouldMarkProductArchivedOnly()
    {
        var product = new ProductBuilder().Build();

        _productRepositoryMock
            .Setup(repository => repository.GetByIdAsync(product.Id))
            .ReturnsAsync(product);

        var handler = new ArchiveProductHandler(_productRepositoryMock.Object);

        var result = await handler.Handle(
            new ArchiveProductCommand { ProductId = product.Id },
            CancellationToken.None);

        result.Should().NotBeNull();
        result!.IsArchived.Should().BeTrue();
        product.IsArchived.Should().BeTrue();
        _productRepositoryMock.Verify(repository => repository.UpdateAsync(product), Times.Once);
    }
}
