using MediatR;

namespace Warehouse.Application.Products.Commands.UpdateProductQuantity;

public class UpdateProductQuantityCommand : IRequest<UpdateProductQuantityResponse?>
{
    public string ProductId { get; set; } = string.Empty;

    public int QuantityInStock { get; set; }
}

