using MediatR;
using Warehouse.Application.ViewModels;

namespace Warehouse.Application.Products.Queries.SearchProducts;

public class SearchProductsQuery : IRequest<List<ProductViewModel>>
{
    public string? Name { get; set; }

    public string? Supplier { get; set; }
}

