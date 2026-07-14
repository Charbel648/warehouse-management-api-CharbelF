using MediatR;
using Warehouse.Application.ViewModels;

namespace Warehouse.Application.Suppliers.Queries.GetSupplierById;

public class GetSupplierByIdQuery : IRequest<SupplierViewModel?>
{
    public string SupplierId { get; set; } = string.Empty;
}
