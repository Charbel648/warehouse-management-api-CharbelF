using MediatR;

namespace Warehouse.Application.Products.Commands.AssignSupplierToProduct;

public class AssignSupplierToProductCommand : IRequest<AssignSupplierToProductResponse?>
{
    public string ProductId { get; set; } = string.Empty;

    public string SupplierId { get; set; } = string.Empty;
}

