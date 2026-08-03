using MediatR;
using Warehouse.Domain.Models;
using Warehouse.Domain.Repositories;

namespace Warehouse.Application.Products.Commands.CreateProduct;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, CreateProductResponse>
{
    private readonly IProductRepository _productRepository;

    public CreateProductHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<CreateProductResponse> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        bool skuExists = await _productRepository.SkuExistsAsync(request.SKU);

        if (skuExists)
            throw new InvalidOperationException("A product with the same SKU already exists");

        Product product = new Product(
            request.Name,
            request.SKU,
            request.Description,
            request.Price,
            request.QuantityInStock,
            request.SupplierName,
            request.ExpiryDate
        );

        await _productRepository.AddAsync(product);

        return new CreateProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            SKU = product.SKU,
            Description = product.Description,
            Price = product.Price,
            QuantityInStock = product.QuantityInStock,
            SupplierName = product.SupplierName,
            ExpiryDate = product.ExpiryDate,
            IsArchived = product.IsArchived,
            CreatedAt = product.CreatedAt,
            LastUpdatedAt = product.LastUpdatedAt
        };
    }
}

