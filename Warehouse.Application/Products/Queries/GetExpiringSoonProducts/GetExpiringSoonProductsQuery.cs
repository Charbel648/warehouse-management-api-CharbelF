using MediatR;

namespace Warehouse.Application.Products.Queries.GetExpiringSoonProducts;

public class GetExpiringSoonProductsQuery : IRequest<List<ExpiringSoonProductDto>>
{
}
