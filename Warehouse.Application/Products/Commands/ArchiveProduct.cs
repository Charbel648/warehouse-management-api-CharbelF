using Warehouse.Application.Contracts;
using Warehouse.Application.Mapping;
using Warehouse.Domain.Repositories;

namespace Warehouse.Application.Products.Commands;

public class ArchiveProduct
{
    private readonly IProductRepository _productRepository;

    public ArchiveProduct(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductDto?> ExecuteAsync(string id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product == null)
        {
            return null;
        }

        product.Archive();

        await _productRepository.UpdateAsync(product);

        return WarehouseMapper.ToDto(product);
    }
}