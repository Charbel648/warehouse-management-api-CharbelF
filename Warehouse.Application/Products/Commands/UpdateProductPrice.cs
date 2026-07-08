using Warehouse.Application.Contracts;
using Warehouse.Application.Mapping;
using Warehouse.Domain.Repositories;

namespace Warehouse.Application.Products.Commands;

public class UpdateProductPrice
{
    private readonly IProductRepository _productRepository;

    public UpdateProductPrice(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductDto?> ExecuteAsync(string id, decimal price)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product == null)
        {
            return null;
        }

        product.UpdatePrice(price);

        await _productRepository.UpdateAsync(product);

        return WarehouseMapper.ToDto(product);
    }
}