using Warehouse.Application.Contracts;
using Warehouse.Application.Mapping;
using Warehouse.Domain.Repositories;

namespace Warehouse.Application.Suppliers.Queries;

public class ListSuppliers
{
    private readonly ISupplierRepository _supplierRepository;

    public ListSuppliers(ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<List<SupplierDto>> ExecuteAsync()
    {
        var suppliers = await _supplierRepository.GetAllAsync();

        return suppliers
            .Select(WarehouseMapper.ToDto)
            .ToList();
    }
}