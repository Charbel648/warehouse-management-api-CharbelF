using Warehouse.Domain.Models;
using Warehouse.Domain.Repositories;
using Warehouse.Infrastructure.Data;

namespace Warehouse.Infrastructure.Repositories;

public class SupplierRepository : ISupplierRepository
{
    public Task<List<Supplier>> GetAllAsync()
    {
        return Task.FromResult(FakeWarehouseStore.Suppliers);
    }

    public Task<Supplier?> GetByIdAsync(string id)
    {
        Supplier? supplier = FakeWarehouseStore.Suppliers
            .FirstOrDefault(s => s.Id == id);

        return Task.FromResult(supplier);
    }

    public Task AddAsync(Supplier supplier)
    {
        FakeWarehouseStore.Suppliers.Add(supplier);

        return Task.CompletedTask;
    }

    public Task UpdateAsync(Supplier supplier)
    {
        return Task.CompletedTask;
    }

    public Task<bool> EmailExistsAsync(string contactEmail)
    {
        bool exists = FakeWarehouseStore.Suppliers.Any(s =>
            s.ContactEmail.Equals(contactEmail, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(exists);
    }
}