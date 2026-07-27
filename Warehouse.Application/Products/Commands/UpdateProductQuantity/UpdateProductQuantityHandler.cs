using Warehouse.Application.Common.Messaging;
using MediatR;
using Warehouse.Domain.Repositories;

namespace Warehouse.Application.Products.Commands.UpdateProductQuantity;

public class UpdateProductQuantityHandler : IRequestHandler<UpdateProductQuantityCommand, UpdateProductQuantityResponse?>
{
    private readonly IProductRepository _productRepository;
    private readonly IWarehouseEventPublisher _warehouseEventPublisher;

    public UpdateProductQuantityHandler(IProductRepository productRepository,
        IWarehouseEventPublisher warehouseEventPublisher)
    {
        _productRepository = productRepository;
        _warehouseEventPublisher = warehouseEventPublisher;
    }

    public async Task<UpdateProductQuantityResponse?> Handle(
        UpdateProductQuantityCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId);

        if (product == null)
            return null;

        product.UpdateQuantity(request.QuantityInStock);

        await _productRepository.UpdateAsync(product);

        
        int lowStockThreshold = 5;

        if (product.QuantityInStock < lowStockThreshold)
        {
            await _warehouseEventPublisher.PublishAsync(
                new WarehouseNotificationEvent
                {
                    EventType = "StockLowDetected",
                    CorrelationId = Guid.NewGuid().ToString(),
                    RelatedEntityId = product.Id,
                    RelatedEntityType = "Product",
                    Severity = "High",
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Quantity = product.QuantityInStock,
                    Threshold = lowStockThreshold
                },
                "stock.low",
                cancellationToken);
        }
return new UpdateProductQuantityResponse
        {
            Id = product.Id,
            Name = product.Name,
            QuantityInStock = product.QuantityInStock,
            LastUpdatedAt = product.LastUpdatedAt
        };
    }
}


