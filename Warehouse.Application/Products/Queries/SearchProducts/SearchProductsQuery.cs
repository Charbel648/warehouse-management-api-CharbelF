using MediatR;

namespace Warehouse.Application.Products.Queries.SearchProducts;

public class SearchProductsQuery : IRequest<List<SearchProductsResponse>>
{
    public string? Name { get; set; }

    public string? Supplier { get; set; }
}
