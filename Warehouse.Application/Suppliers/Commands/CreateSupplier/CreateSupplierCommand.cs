using MediatR;

namespace Warehouse.Application.Suppliers.Commands.CreateSupplier;

public class CreateSupplierCommand : IRequest<CreateSupplierResponse>
{
    public string Name { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public string ContactEmail { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;
}
