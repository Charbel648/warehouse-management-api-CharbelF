using MediatR;
using Warehouse.Domain.Repositories;

namespace Warehouse.Application.Products.Queries.SearchProducts;

public class SearchProductsHandler : IRequestHandler<SearchProductsQuery, List<SearchProductsResponse>>
{
    private readonly IProductRepository _productRepository;

    public SearchProductsHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<List<SearchProductsResponse>> Handle(
        SearchProductsQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name) && string.IsNullOrWhiteSpace(request.Supplier))
            throw new ArgumentException("You must provide name or supplier");

        var products = await _productRepository.SearchAsync(request.Name, request.Supplier);

        return products
            .Select(p => new SearchProductsResponse
            {
                Id = p.Id,
                Name = p.Name,
                SKU = p.SKU,
                Price = p.Price,
                QuantityInStock = p.QuantityInStock,
                SupplierName = p.SupplierName
            })
            .ToList();
    }
}
