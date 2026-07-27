using MediatR;
using Microsoft.AspNetCore.Authorization;
using Warehouse.Application.Files.Commands.UploadSupplierDocument;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Application.Suppliers.Commands.CreateSupplier;
using Warehouse.Application.Suppliers.Commands.DeactivateSupplier;
using Warehouse.Application.Suppliers.Queries.GetSupplierById;
using Warehouse.Application.Suppliers.Queries.ListSuppliers;
using Warehouse.Domain.Exceptions;
using Warehouse.Presentation.Contracts;
using Warehouse.Presentation.Security;

namespace Warehouse.Presentation.Controllers;

[ApiController]
[Route("api/suppliers")]
public class SuppliersController : ControllerBase
{
    private readonly IMediator _mediator;

    public SuppliersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Policy = WarehousePolicies.WarehouseReader)]
    [HttpGet]
    public async Task<ActionResult> GetSuppliers(CancellationToken cancellationToken)
    {
        var suppliers = await _mediator.Send(new ListSuppliersQuery(), cancellationToken);

        return Ok(suppliers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetSupplier(
        [FromRoute] string id,
        CancellationToken cancellationToken)
    {
        ValidateGuid(id, "supplier");

        var supplier = await _mediator.Send(new GetSupplierByIdQuery
        {
            SupplierId = id
        }, cancellationToken);

        if (supplier == null)
            throw new NotFoundException("Supplier", id);

        return Ok(supplier);
    }

    [Authorize(Policy = WarehousePolicies.WarehouseAdmin)]
    [HttpPost]
    public async Task<ActionResult> AddSupplier(
        [FromBody] CreateSupplierRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateSupplierCommand
        {
            Name = request.Name,
            Country = request.Country,
            ContactEmail = request.ContactEmail,
            PhoneNumber = request.PhoneNumber
        }, cancellationToken);

        return CreatedAtAction(nameof(GetSupplier), new { id = response.Id }, response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeactivateSupplier(
        [FromRoute] string id,
        CancellationToken cancellationToken)
    {
        ValidateGuid(id, "supplier");

        var supplier = await _mediator.Send(new DeactivateSupplierCommand
        {
            SupplierId = id
        }, cancellationToken);

        if (supplier == null)
            throw new NotFoundException("Supplier", id);

        return Ok(supplier);
    }

    private static void ValidateGuid(string id, string resourceName)
    {
        if (!Guid.TryParse(id, out _))
            throw new BusinessRuleException($"Invalid {resourceName} id", $"invalid_{resourceName}_id");
    }

    [Authorize(Policy = WarehousePolicies.WarehouseAdmin)]
    [HttpPost("{id:guid}/documents")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult> UploadSupplierDocument(
        [FromRoute] Guid id,
        [FromForm] UploadSupplierDocumentRequest request,
        CancellationToken cancellationToken)
    {
        await using Stream content = request.Document.OpenReadStream();

        var response = await _mediator.Send(new UploadSupplierDocumentCommand
        {
            SupplierId = id.ToString(),
            Content = content,
            FileName = request.Document.FileName,
            ContentType = request.Document.ContentType,
            SizeInBytes = request.Document.Length
        }, cancellationToken);

        return Ok(response);
    }
}






