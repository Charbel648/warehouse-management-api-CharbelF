using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Application.StockAdjustments.Commands.AdjustStock;
using Warehouse.Presentation.Contracts;
using Warehouse.Presentation.Security;

namespace Warehouse.Presentation.Controllers;

[ApiController]
[Route("api/stock-adjustments")]
public class StockAdjustmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public StockAdjustmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Policy = WarehousePolicies.WarehouseAdmin)]
    [HttpPost]
    public async Task<ActionResult> AdjustStock(
        [FromBody] StockAdjustmentRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new AdjustStockCommand
        {
            ProductId = request.ProductId,
            QuantityChange = request.QuantityChange,
            Reason = request.Reason
        }, cancellationToken);

        return Ok(response);
    }
}

