using MediatR;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Application.Suppliers.Commands.CreateSupplier;
using Warehouse.Application.Suppliers.Commands.DeactivateSupplier;
using Warehouse.Application.Suppliers.Queries.GetSupplierById;
using Warehouse.Application.Suppliers.Queries.ListSuppliers;
using Warehouse.Presentation.Contracts;

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

    [HttpGet]
    public async Task<ActionResult> GetSuppliers()
    {
        var suppliers = await _mediator.Send(new ListSuppliersQuery());

        return Ok(suppliers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetSupplier([FromRoute] string id)
    {
        if (!Guid.TryParse(id, out _))
            return BadRequest("Invalid supplier id");

        var supplier = await _mediator.Send(new GetSupplierByIdQuery
        {
            SupplierId = id
        });

        if (supplier == null)
            return NotFound("Supplier not found");

        return Ok(supplier);
    }

    [HttpPost]
    public async Task<ActionResult> AddSupplier([FromBody] CreateSupplierRequest request)
    {
        try
        {
            var response = await _mediator.Send(new CreateSupplierCommand
            {
                Name = request.Name,
                Country = request.Country,
                ContactEmail = request.ContactEmail,
                PhoneNumber = request.PhoneNumber
            });

            return CreatedAtAction(nameof(GetSupplier), new { id = response.Id }, response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeactivateSupplier([FromRoute] string id)
    {
        if (!Guid.TryParse(id, out _))
            return BadRequest("Invalid supplier id");

        var supplier = await _mediator.Send(new DeactivateSupplierCommand
        {
            SupplierId = id
        });

        if (supplier == null)
            return NotFound("Supplier not found");

        return Ok(supplier);
    }
}
