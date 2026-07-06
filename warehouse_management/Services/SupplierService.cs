using warehouse_management.Contracts;
using warehouse_management.Data;
using warehouse_management.Models;

namespace warehouse_management.Services;

public class SupplierService
{
    public async Task<List<Supplier>> GetAllSuppliers()
    {
        return await Task.FromResult(FakeWarehouseStore.DummySuppliers);
    }

    public async Task<Supplier?> GetSupplierById(string id)
    {
        Supplier? supplier = FakeWarehouseStore.DummySuppliers
            .FirstOrDefault(s => s.Id == id);

        return await Task.FromResult(supplier);
    }

    public async Task<Supplier> CreateSupplier(CreateSupplierRequest request)
    {
        Supplier supplier = new Supplier
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Name,
            Country = request.Country,
            ContactEmail = request.ContactEmail,
            PhoneNumber = request.PhoneNumber,
            IsActive = true
        };

        FakeWarehouseStore.DummySuppliers.Add(supplier);

        return await Task.FromResult(supplier);
    }

    public async Task<Supplier?> DeactivateSupplier(string id)
    {
        Supplier? supplier = FakeWarehouseStore.DummySuppliers
            .FirstOrDefault(s => s.Id == id);

        if (supplier == null)
        {
            return null;
        }

        supplier.IsActive = false;

        return await Task.FromResult(supplier);
    }
}