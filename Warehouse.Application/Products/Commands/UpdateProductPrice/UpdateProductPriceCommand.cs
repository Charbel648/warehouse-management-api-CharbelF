using MediatR;

namespace Warehouse.Application.Products.Commands.UpdateProductPrice;

public class UpdateProductPriceCommand : IRequest<UpdateProductPriceResponse?>
{
    public string ProductId { get; set; } = string.Empty;

    public decimal Price { get; set; }
}

