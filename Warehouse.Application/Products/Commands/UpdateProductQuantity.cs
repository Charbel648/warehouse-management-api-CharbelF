using Warehouse.Application.Contracts;
using Warehouse.Application.Mapping;
using Warehouse.Domain.Repositories;

namespace Warehouse.Application.Products.Commands;

public class UpdateProductQuantity
{
    private readonly IProductRepository _productRepository;

    public UpdateProductQuantity(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductDto?> ExecuteAsync(string id, int quantity)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product == null)
        {
            return null;
        }

        product.UpdateQuantity(quantity);

        await _productRepository.UpdateAsync(product);

        return WarehouseMapper.ToDto(product);
    }
}