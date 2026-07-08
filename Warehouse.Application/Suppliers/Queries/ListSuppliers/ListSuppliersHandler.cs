using MediatR;
using Warehouse.Domain.Repositories;

namespace Warehouse.Application.Suppliers.Queries.ListSuppliers;

public class ListSuppliersHandler : IRequestHandler<ListSuppliersQuery, List<ListSuppliersResponse>>
{
    private readonly ISupplierRepository _supplierRepository;

    public ListSuppliersHandler(ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<List<ListSuppliersResponse>> Handle(
        ListSuppliersQuery request,
        CancellationToken cancellationToken)
    {
        var suppliers = await _supplierRepository.GetAllAsync();

        return suppliers
            .Select(s => new ListSuppliersResponse
            {
                Id = s.Id,
                Name = s.Name,
                Country = s.Country,
                ContactEmail = s.ContactEmail,
                PhoneNumber = s.PhoneNumber,
                IsActive = s.IsActive
            })
            .ToList();
    }
}
