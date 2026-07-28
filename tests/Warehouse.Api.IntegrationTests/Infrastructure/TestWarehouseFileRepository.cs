using Warehouse.Domain.Models;
using Warehouse.Domain.Repositories;

namespace Warehouse.Api.IntegrationTests.Infrastructure;

public class TestWarehouseFileRepository : IWarehouseFileRepository
{
    private readonly TestWarehouseStore _store;

    public TestWarehouseFileRepository(TestWarehouseStore store)
    {
        _store = store;
    }

    public Task AddAsync(WarehouseFile file, CancellationToken cancellationToken)
    {
        _store.Files.Add(file);
        return Task.CompletedTask;
    }

    public Task<WarehouseFile?> GetByIdAsync(string fileId, CancellationToken cancellationToken)
    {
        return Task.FromResult(_store.Files.FirstOrDefault(file => file.FileId == fileId));
    }
}
