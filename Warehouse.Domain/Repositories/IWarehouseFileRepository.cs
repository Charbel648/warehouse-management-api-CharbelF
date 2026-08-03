using Warehouse.Domain.Models;

namespace Warehouse.Domain.Repositories;

public interface IWarehouseFileRepository
{
    Task AddAsync(WarehouseFile file, CancellationToken cancellationToken);

    Task<WarehouseFile?> GetByIdAsync(string fileId, CancellationToken cancellationToken);
}

