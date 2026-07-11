using MediatR;
using Warehouse.Application.ViewModels;

namespace Warehouse.Application.Products.Queries.GetProductById;

public class GetProductByIdQuery : IRequest<ProductViewModel?>
{
    public string ProductId { get; set; } = string.Empty;
}
