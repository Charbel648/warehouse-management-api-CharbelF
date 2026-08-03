using MediatR;
using Warehouse.Application.ViewModels;

namespace Warehouse.Application.Products.Queries.ListProducts;

public class ListProductsQuery : IRequest<List<ProductViewModel>>
{
    public bool OnlyAvailable { get; set; }
}

