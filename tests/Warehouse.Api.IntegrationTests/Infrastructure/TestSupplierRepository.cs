using Warehouse.Domain.Models;
using Warehouse.Domain.Repositories;

namespace Warehouse.Api.IntegrationTests.Infrastructure;

public class TestSupplierRepository : ISupplierRepository
{
    private readonly TestWarehouseStore _store;

    public TestSupplierRepository(TestWarehouseStore store)
    {
        _store = store;
    }

    public Task<List<Supplier>> GetAllAsync()
    {
        return Task.FromResult(_store.Suppliers.ToList());
    }

    public Task<Supplier?> GetByIdAsync(string id)
    {
        return Task.FromResult(_store.Suppliers.FirstOrDefault(supplier => supplier.Id == id));
    }

    public Task AddAsync(Supplier supplier)
    {
        _store.Suppliers.Add(supplier);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Supplier supplier)
    {
        return Task.CompletedTask;
    }

    public Task<bool> EmailExistsAsync(string contactEmail)
    {
        bool exists = _store.Suppliers.Any(supplier =>
            supplier.ContactEmail.Equals(contactEmail, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(exists);
    }
}
