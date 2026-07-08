using MediatR;

namespace Warehouse.Application.Suppliers.Queries.GetSupplierById;

public class GetSupplierByIdQuery : IRequest<GetSupplierByIdResponse?>
{
    public string SupplierId { get; set; } = string.Empty;
}
