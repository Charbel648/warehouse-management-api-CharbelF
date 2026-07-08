using Warehouse.Application.Contracts;
using Warehouse.Application.Mapping;
using Warehouse.Domain.Repositories;

namespace Warehouse.Application.Products.Queries;

public class ListProducts
{
    private readonly IProductRepository _productRepository;

    public ListProducts(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<List<ProductDto>> ExecuteAsync(bool onlyAvailable)
    {
        var products = await _productRepository.GetAllAsync();

        if (onlyAvailable)
        {
            products = products
                .Where(p => !p.IsArchived && p.QuantityInStock > 0)
                .ToList();
        }

        return products
            .OrderByDescending(p => p.CreatedAt)
            .Select(WarehouseMapper.ToDto)
            .ToList();
    }
}