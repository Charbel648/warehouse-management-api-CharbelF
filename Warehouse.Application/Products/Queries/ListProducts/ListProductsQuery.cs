using MediatR;

namespace Warehouse.Application.Products.Queries.ListProducts;

public class ListProductsQuery : IRequest<List<ListProductsResponse>>
{
    public bool OnlyAvailable { get; set; }
}
