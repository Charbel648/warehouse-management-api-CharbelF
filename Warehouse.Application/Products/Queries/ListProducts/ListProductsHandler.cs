using MediatR;
using Warehouse.Domain.Repositories;

namespace Warehouse.Application.Products.Queries.ListProducts;

public class ListProductsHandler : IRequestHandler<ListProductsQuery, List<ListProductsResponse>>
{
    private readonly IProductRepository _productRepository;

    public ListProductsHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<List<ListProductsResponse>> Handle(
        ListProductsQuery request,
        CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetAllAsync();

        if (request.OnlyAvailable)
        {
            products = products
                .Where(p => !p.IsArchived && p.QuantityInStock > 0)
                .ToList();
        }

        return products
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new ListProductsResponse
            {
                Id = p.Id,
                Name = p.Name,
                SKU = p.SKU,
                Description = p.Description,
                Price = p.Price,
                QuantityInStock = p.QuantityInStock,
                SupplierName = p.SupplierName,
                IsArchived = p.IsArchived,
                CreatedAt = p.CreatedAt
            })
            .ToList();
    }
}
