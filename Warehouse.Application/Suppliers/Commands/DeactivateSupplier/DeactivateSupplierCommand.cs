using MediatR;

namespace Warehouse.Application.Suppliers.Commands.DeactivateSupplier;

public class DeactivateSupplierCommand : IRequest<DeactivateSupplierResponse?>
{
    public string SupplierId { get; set; } = string.Empty;
}
