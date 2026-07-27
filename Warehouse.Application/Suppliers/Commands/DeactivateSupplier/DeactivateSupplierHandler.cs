using MediatR;
using Warehouse.Domain.Repositories;

namespace Warehouse.Application.Suppliers.Commands.DeactivateSupplier;

public class DeactivateSupplierHandler : IRequestHandler<DeactivateSupplierCommand, DeactivateSupplierResponse?>
{
    private readonly ISupplierRepository _supplierRepository;

    public DeactivateSupplierHandler(ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<DeactivateSupplierResponse?> Handle(
        DeactivateSupplierCommand request,
        CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.GetByIdAsync(request.SupplierId);

        if (supplier == null)
            return null;

        supplier.Deactivate();

        await _supplierRepository.UpdateAsync(supplier);

        return new DeactivateSupplierResponse
        {
            Id = supplier.Id,
            Name = supplier.Name,
            IsActive = supplier.IsActive
        };
    }
}

