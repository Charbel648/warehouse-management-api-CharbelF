using MediatR;
using Warehouse.Domain.Repositories;

namespace Warehouse.Application.Suppliers.Queries.GetSupplierById;

public class GetSupplierByIdHandler : IRequestHandler<GetSupplierByIdQuery, GetSupplierByIdResponse?>
{
    private readonly ISupplierRepository _supplierRepository;

    public GetSupplierByIdHandler(ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<GetSupplierByIdResponse?> Handle(
        GetSupplierByIdQuery request,
        CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.GetByIdAsync(request.SupplierId);

        if (supplier == null)
            return null;

        return new GetSupplierByIdResponse
        {
            Id = supplier.Id,
            Name = supplier.Name,
            Country = supplier.Country,
            ContactEmail = supplier.ContactEmail,
            PhoneNumber = supplier.PhoneNumber,
            IsActive = supplier.IsActive
        };
    }
}
