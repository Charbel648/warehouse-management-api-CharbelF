using MediatR;
using Warehouse.Domain.Exceptions;
using Warehouse.Domain.Repositories;

namespace Warehouse.Application.StockAdjustments.Commands.AdjustStock;

public class AdjustStockCommandHandler : IRequestHandler<AdjustStockCommand, AdjustStockResponse>
{
    private readonly IProductRepository _productRepository;

    public AdjustStockCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
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
