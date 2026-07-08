using MediatR;

namespace Warehouse.Application.Suppliers.Queries.ListSuppliers;

public class ListSuppliersQuery : IRequest<List<ListSuppliersResponse>>
{
}
