using Warehouse.Domain.Models;

namespace Warehouse.Domain.Repositories;

public interface IProductReportRepository
{
    Task<List<Product>> GetProductsBySupplierAsync(string supplierName, string sortOrder);

    Task<int> GetTotalProductsCountAsync();

    Task<List<Product>> GetPagedProductsAsync(int pageNumber, int pageSize);
}

