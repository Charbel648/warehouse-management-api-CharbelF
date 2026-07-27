using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Application.Inventory.Queries.GetInventoryDashboard;
using Warehouse.Presentation.Security;

namespace Warehouse.Presentation.Controllers;

[ApiController]
[Route("api/inventory")]
public class InventoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public InventoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Policy = WarehousePolicies.WarehouseReader)]
    [HttpGet("dashboard")]
    public async Task<ActionResult> GetDashboard(CancellationToken cancellationToken)
    {
        var dashboard = await _mediator.Send(new GetInventoryDashboardQuery(), cancellationToken);

        return Ok(dashboard);
    }
}


