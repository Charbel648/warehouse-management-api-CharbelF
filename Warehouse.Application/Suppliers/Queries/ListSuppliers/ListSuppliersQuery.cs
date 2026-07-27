using MediatR;
using Warehouse.Application.ViewModels;

namespace Warehouse.Application.Suppliers.Queries.ListSuppliers;

public class ListSuppliersQuery : IRequest<List<SupplierViewModel>>
{
}

