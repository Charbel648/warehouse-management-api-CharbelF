using MediatR;
using Warehouse.Domain.Repositories;

namespace Warehouse.Application.Products.Queries.GetExpiringSoonProducts;

public class GetExpiringSoonProductsHandler
    : IRequestHandler<GetExpiringSoonProductsQuery, List<ExpiringSoonProductDto>>
{
    private const int ExpiringSoonWindowInDays = 30;

    private readonly IProductRepository _productRepository;

    public GetExpiringSoonProductsHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<List<ExpiringSoonProductDto>> Handle(
        GetExpiringSoonProductsQuery request,
        CancellationToken cancellationToken)
    {
        DateTime currentDate = DateTime.UtcNow.Date;
        DateTime expiringLimitDate = currentDate.AddDays(ExpiringSoonWindowInDays);

        var products = await _productRepository.GetExpiredOrExpiringProductsAsync(
            currentDate,
            expiringLimitDate,
            cancellationToken);

        return products
            .Select(product => new ExpiringSoonProductDto
            {
                Id = product.Id,
                Name = product.Name,
                SKU = product.SKU,
                QuantityInStock = product.QuantityInStock,
                SupplierName = product.SupplierName,
                ExpiryDate = product.ExpiryDate,
                DaysUntilExpiry = (product.ExpiryDate.Date - currentDate).Days
            })
            .ToList();
    }
}
