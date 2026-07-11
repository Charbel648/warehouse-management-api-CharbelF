using Warehouse.Domain.Models;

namespace Warehouse.Domain.Repositories;

public interface ISupplierRepository
{
    Task<List<Supplier>> GetAllAsync();

    Task<Supplier?> GetByIdAsync(string id);

    Task AddAsync(Supplier supplier);

    Task UpdateAsync(Supplier supplier);

    Task<bool> EmailExistsAsync(string contactEmail);
}
