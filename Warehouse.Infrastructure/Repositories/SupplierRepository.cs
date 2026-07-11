using Microsoft.EntityFrameworkCore;
using Warehouse.Domain.Models;
using Warehouse.Domain.Repositories;
using Warehouse.Infrastructure.Persistence;

namespace Warehouse.Infrastructure.Repositories;

public class SupplierRepository : ISupplierRepository
{
    private readonly WarehouseDbContext _context;

    public SupplierRepository(WarehouseDbContext context)
    {
        _context = context;
    }

    public async Task<List<Supplier>> GetAllAsync()
    {
        return await _context.Suppliers
            .Include(s => s.Products)
            .ToListAsync();
    }

    public async Task<Supplier?> GetByIdAsync(string id)
    {
        return await _context.Suppliers
            .Include(s => s.Products)
            .FirstOrDefaultAsync(s => s.SupplierId == id);
    }

    public async Task AddAsync(Supplier supplier)
    {
        await _context.Suppliers.AddAsync(supplier);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Supplier supplier)
    {
        _context.Suppliers.Update(supplier);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> EmailExistsAsync(string contactEmail)
    {
        return await _context.Suppliers
            .AnyAsync(s => s.ContactEmail.ToLower() == contactEmail.ToLower());
    }
}
