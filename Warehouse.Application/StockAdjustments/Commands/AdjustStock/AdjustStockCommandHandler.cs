using MediatR;
using Warehouse.Application.Common;
using Warehouse.Application.Common.Caching;
using Warehouse.Domain.Exceptions;
using Warehouse.Domain.Repositories;

namespace Warehouse.Application.StockAdjustments.Commands.AdjustStock;

public class AdjustStockCommandHandler : IRequestHandler<AdjustStockCommand, AdjustStockResponse>
{
    private readonly IProductRepository _productRepository;
    private readonly ICacheService _cacheService;

    public AdjustStockCommandHandler(
        IProductRepository productRepository,
        ICacheService cacheService)
    {
        _productRepository = productRepository;
        _cacheService = cacheService;
    }

    public async Task<AdjustStockResponse> Handle(
        AdjustStockCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId);

        if (product == null)
            throw new NotFoundException("Product", request.ProductId);

        int previousQuantity = product.QuantityInStock;
        int newQuantity = previousQuantity + request.QuantityChange;

        if (newQuantity < 0)
            throw new BusinessRuleException("Stock adjustment cannot make product quantity negative");

        product.UpdateQuantity(newQuantity);

        await _productRepository.UpdateAsync(product);

        await WarehouseCacheInvalidator.InvalidateProductCachesAsync(
            _cacheService,
            cancellationToken);

        return new AdjustStockResponse
        {
            ProductId = product.Id,
            ProductName = product.Name,
            PreviousQuantity = previousQuantity,
            QuantityChange = request.QuantityChange,
            NewQuantity = product.QuantityInStock,
            Reason = request.Reason,
            UpdatedAt = product.LastUpdatedAt
        };
    }
}
