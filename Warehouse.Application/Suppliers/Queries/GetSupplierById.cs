using Warehouse.Application.Contracts;
using Warehouse.Application.Mapping;
using Warehouse.Domain.Repositories;

namespace Warehouse.Application.Suppliers.Queries;

public class GetSupplierById
{
    private readonly ISupplierRepository _supplierRepository;

    public GetSupplierById(ISupplierRepository supplierRepository)
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

        return WarehouseMapper.ToDto(supplier);
    }
}