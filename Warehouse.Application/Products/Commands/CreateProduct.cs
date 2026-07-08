using Warehouse.Application.Contracts;
using Warehouse.Application.Mapping;
using Warehouse.Domain.Models;
using Warehouse.Domain.Repositories;

namespace Warehouse.Application.Products.Commands;

public class CreateProduct
{
    private readonly IProductRepository _productRepository;

    public CreateProduct(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductDto> ExecuteAsync(
        string name,
        string sku,
        string description,
        decimal price,
        int quantityInStock,
        string supplierName,
        DateTime expiryDate)
    {
        bool skuExists = await _productRepository.SkuExistsAsync(sku);

        if (skuExists)
        {
            throw new InvalidOperationException("A product with the same SKU already exists");
        }

        Product product = new Product(
            name,
            sku,
            description,
            price,
            quantityInStock,
            supplierName,
            expiryDate
        );

        await _productRepository.AddAsync(product);

        return WarehouseMapper.ToDto(product);
    }
}