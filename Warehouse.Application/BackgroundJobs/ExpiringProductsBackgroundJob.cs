using Microsoft.Extensions.Logging;
using Warehouse.Domain.Repositories;

namespace Warehouse.Application.BackgroundJobs;

public class ExpiringProductsBackgroundJob
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<ExpiringProductsBackgroundJob> _logger;

    public ExpiringProductsBackgroundJob(
        IProductRepository productRepository,
        ILogger<ExpiringProductsBackgroundJob> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task CheckExpiringProductsAsync()
    {
        DateTime currentDate = DateTime.UtcNow.Date;
        DateTime expiringLimitDate = currentDate.AddDays(30);

        var products = await _productRepository.GetExpiredOrExpiringProductsAsync(
            currentDate,
            expiringLimitDate,
            CancellationToken.None);

        var expiredProducts = products
            .Where(product => product.ExpiryDate.Date < currentDate)
            .ToList();

        var soonToExpireProducts = products
            .Where(product =>
                product.ExpiryDate.Date >= currentDate
                && product.ExpiryDate.Date <= expiringLimitDate)
            .ToList();

        _logger.LogInformation(
            "Expired products check completed. ExpiredCount: {ExpiredCount}, SoonToExpireCount: {SoonToExpireCount}",
            expiredProducts.Count,
            soonToExpireProducts.Count);

        foreach (var product in expiredProducts)
        {
            _logger.LogWarning(
                "Expired product found. ProductId: {ProductId}, ProductName: {ProductName}, ExpiryDate: {ExpiryDate}",
                product.Id,
                product.Name,
                product.ExpiryDate);
        }

        foreach (var product in soonToExpireProducts)
        {
            _logger.LogInformation(
                "Soon-to-expire product found. ProductId: {ProductId}, ProductName: {ProductName}, ExpiryDate: {ExpiryDate}",
                product.Id,
                product.Name,
                product.ExpiryDate);
        }
    }
}
