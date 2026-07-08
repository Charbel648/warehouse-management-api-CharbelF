using Warehouse.Application.Contracts;
using Warehouse.Application.Mapping;
using Warehouse.Domain.Repositories;

namespace Warehouse.Application.Products.Queries;

public class GetProductById
{
    private readonly IProductRepository _productRepository;

    public GetProductById(IProductRepository productRepository)
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

        return WarehouseMapper.ToDto(product);
    }
}