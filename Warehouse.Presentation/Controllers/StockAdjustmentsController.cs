using Microsoft.AspNetCore.Mvc;
using Warehouse.Domain.Exceptions;
using Warehouse.Domain.Repositories;
using Warehouse.Presentation.Contracts;

namespace Warehouse.Presentation.Controllers;

[ApiController]
[Route("api/stock-adjustments")]
public class StockAdjustmentsController : ControllerBase
{
    private readonly IProductRepository _productRepository;

    public StockAdjustmentsController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    [HttpPost]
    public async Task<ActionResult> AdjustStock(
        [FromBody] StockAdjustmentRequest request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.ProductId, out _))
            throw new BusinessRuleException("Invalid product id", "invalid_product_id");

        var product = await _productRepository.GetByIdAsync(request.ProductId);

        if (product == null)
            throw new NotFoundException("Product", request.ProductId);

        int previousQuantity = product.QuantityInStock;
        int newQuantity = previousQuantity + request.QuantityChange;

        if (newQuantity < 0)
            throw new BusinessRuleException("Stock adjustment cannot make product quantity negative");

        product.UpdateQuantity(newQuantity);

        await _productRepository.UpdateAsync(product);

        return Ok(new
        {
            ProductId = product.Id,
            ProductName = product.Name,
            PreviousQuantity = previousQuantity,
            QuantityChange = request.QuantityChange,
            NewQuantity = product.QuantityInStock,
            request.Reason,
            UpdatedAt = product.LastUpdatedAt
        });
    }
}
