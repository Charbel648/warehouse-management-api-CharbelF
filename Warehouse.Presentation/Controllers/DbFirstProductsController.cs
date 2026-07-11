using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Warehouse.Infrastructure.Persistence.DbFirst;

namespace Warehouse.Presentation.Controllers;

[ApiController]
[Route("api/db-first/products")]
public class DbFirstProductsController : ControllerBase
{
    private readonly WarehouseDbFirstContext _context;

    public DbFirstProductsController(WarehouseDbFirstContext context)
    {
        _context = context;
    }

    [HttpGet("by-supplier")]
    public async Task<ActionResult> GetProductsBySupplier(
        [FromQuery] string supplierName,
        [FromQuery] string sortOrder = "asc")
    {
        if (string.IsNullOrWhiteSpace(supplierName))
            return BadRequest("Supplier name is required");

        if (sortOrder.ToLower() != "asc" && sortOrder.ToLower() != "desc")
            return BadRequest("Sort order must be asc or desc");

        var query = _context.Products
            .Include(product => product.Supplier)
            .Where(product =>
                product.Supplier != null &&
                product.Supplier.Name.ToLower() == supplierName.ToLower());

        if (sortOrder.ToLower() == "asc")
        {
            query = query.OrderBy(product => product.CreatedAt);
        }
        else
        {
            query = query.OrderByDescending(product => product.CreatedAt);
        }

        var products = await query
            .Select(product => new
            {
                product.ProductId,
                product.Name,
                product.Description,
                product.Price,
                product.QuantityInStock,
                product.ExpiryDate,
                product.IsArchived,
                product.CreatedAt,
                product.LastUpdatedAt,
                Supplier = product.Supplier == null ? null : new
                {
                    product.Supplier.SupplierId,
                    product.Supplier.Name,
                    product.Supplier.Country,
                    product.Supplier.ContactEmail,
                    product.Supplier.PhoneNumber,
                    product.Supplier.IsActive
                }
            })
            .ToListAsync();

        return Ok(products);
    }

    [HttpGet("group-by-expiry-year")]
    public async Task<ActionResult> GroupProductsByExpiryYear()
    {
        var result = await _context.Products
            .GroupBy(product => product.ExpiryDate.Year)
            .Select(group => new
            {
                ExpiryYear = group.Key,
                ProductCount = group.Count()
            })
            .OrderBy(group => group.ExpiryYear)
            .ToListAsync();

        return Ok(result);
    }

    [HttpGet("group-by-expiry-year-and-country")]
    public async Task<ActionResult> GroupProductsByExpiryYearAndCountry()
    {
        var result = await _context.Products
            .Include(product => product.Supplier)
            .GroupBy(product => new
            {
                ExpiryYear = product.ExpiryDate.Year,
                SupplierCountry = product.Supplier == null
                    ? "No supplier"
                    : product.Supplier.Country
            })
            .Select(group => new
            {
                group.Key.ExpiryYear,
                group.Key.SupplierCountry,
                ProductCount = group.Count()
            })
            .OrderBy(group => group.ExpiryYear)
            .ThenBy(group => group.SupplierCountry)
            .ToListAsync();

        return Ok(result);
    }

    [HttpGet("count")]
    public async Task<ActionResult> GetTotalProductsCount()
    {
        int count = await _context.Products.CountAsync();

        return Ok(new
        {
            TotalProducts = count
        });
    }

    [HttpGet("paged")]
    public async Task<ActionResult> GetPagedProducts(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 5)
    {
        if (pageNumber < 1)
            return BadRequest("Page number must be greater than zero");

        if (pageSize < 1)
            return BadRequest("Page size must be greater than zero");

        int skip = (pageNumber - 1) * pageSize;

        int totalProducts = await _context.Products.CountAsync();

        var products = await _context.Products
            .Include(product => product.Supplier)
            .OrderBy(product => product.CreatedAt)
            .Skip(skip)
            .Take(pageSize)
            .Select(product => new
            {
                product.ProductId,
                product.Name,
                product.Description,
                product.Price,
                product.QuantityInStock,
                product.ExpiryDate,
                product.IsArchived,
                product.CreatedAt,
                SupplierName = product.Supplier == null ? null : product.Supplier.Name,
                SupplierCountry = product.Supplier == null ? null : product.Supplier.Country
            })
            .ToListAsync();

        return Ok(new
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalProducts = totalProducts,
            Products = products
        });
    }
}
