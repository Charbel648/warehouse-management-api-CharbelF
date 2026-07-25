using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Application.Files.Queries.DownloadWarehouseFile;
using Warehouse.Presentation.Security;

namespace Warehouse.Presentation.Controllers;

[ApiController]
[Route("api/files")]
public class WarehouseFilesController : ControllerBase
{
    private readonly IMediator _mediator;

    public WarehouseFilesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Policy = WarehousePolicies.WarehouseReader)]
    [HttpGet("{fileId:guid}/download")]
    public async Task<IActionResult> DownloadFile(
        [FromRoute] Guid fileId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DownloadWarehouseFileQuery
        {
            FileId = fileId.ToString()
        }, cancellationToken);

        return File(result.Content, result.ContentType, result.FileName);
    }
}
