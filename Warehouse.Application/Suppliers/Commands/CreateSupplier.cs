using Warehouse.Application.Contracts;
using Warehouse.Application.Mapping;
using Warehouse.Domain.Models;
using Warehouse.Domain.Repositories;

namespace Warehouse.Application.Suppliers.Commands;

public class CreateSupplier
{
    private readonly ISupplierRepository _supplierRepository;

    public CreateSupplier(ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<SupplierDto> ExecuteAsync(
        string name,
        string country,
        string contactEmail,
        string phoneNumber)
    {
        bool emailExists = await _supplierRepository.EmailExistsAsync(contactEmail);

        if (emailExists)
        {
            throw new InvalidOperationException("A supplier with the same email already exists");
        }

        Supplier supplier = new Supplier(
            name,
            country,
            contactEmail,
            phoneNumber
        );

        await _supplierRepository.AddAsync(supplier);

        return WarehouseMapper.ToDto(supplier);
    }
}