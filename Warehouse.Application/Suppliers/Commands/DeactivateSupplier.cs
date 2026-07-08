using Warehouse.Application.Contracts;
using Warehouse.Application.Mapping;
using Warehouse.Domain.Repositories;

namespace Warehouse.Application.Suppliers.Commands;

public class DeactivateSupplier
{
    private readonly ISupplierRepository _supplierRepository;

    public DeactivateSupplier(ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<SupplierDto?> ExecuteAsync(string id)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id);

        if (supplier == null)
        {
            return null;
        }

        supplier.Deactivate();

        await _supplierRepository.UpdateAsync(supplier);

        return WarehouseMapper.ToDto(supplier);
    }
}