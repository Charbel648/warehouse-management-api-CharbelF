using Microsoft.EntityFrameworkCore;
using Warehouse.Domain.Models;
using Warehouse.Domain.Repositories;
using Warehouse.Infrastructure.Persistence;

namespace Warehouse.Infrastructure.Repositories;

public class WarehouseFileRepository : IWarehouseFileRepository
{
    private readonly WarehouseDbContext _context;

    public WarehouseFileRepository(WarehouseDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(WarehouseFile file, CancellationToken cancellationToken)
    {
        await _context.WarehouseFiles.AddAsync(file, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<WarehouseFile?> GetByIdAsync(string fileId, CancellationToken cancellationToken)
    {
        return await _context.WarehouseFiles
            .FirstOrDefaultAsync(file => file.FileId == fileId, cancellationToken);
    }
}
