using Warehouse.Application.Contracts;
using Warehouse.Application.Mapping;
using Warehouse.Domain.Repositories;

namespace Warehouse.Application.Products.Queries;

public class SearchProducts
{
    private readonly IProductRepository _productRepository;

    public SearchProducts(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<List<ProductDto>> ExecuteAsync(string? name, string? supplier)
    {
        if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(supplier))
        {
            throw new ArgumentException("You must provide name or supplier");
        }

        var products = await _productRepository.SearchAsync(name, supplier);

        return products
            .Select(WarehouseMapper.ToDto)
            .ToList();
    }
}