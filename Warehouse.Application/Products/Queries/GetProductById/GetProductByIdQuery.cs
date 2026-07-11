using MediatR;

namespace Warehouse.Application.Products.Queries.GetProductById;

public class GetProductByIdQuery : IRequest<GetProductByIdResponse?>
{
    public string ProductId { get; set; } = string.Empty;
}
