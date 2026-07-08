using Microsoft.AspNetCore.Mvc;
using Warehouse.Application.Suppliers.Commands;
using Warehouse.Application.Suppliers.Queries;
using Warehouse.Presentation.Contracts;

namespace Warehouse.Presentation.Controllers;

[ApiController]
[Route("api/suppliers")]
public class SuppliersController : ControllerBase
{
    private readonly ListSuppliers _listSuppliers;
    private readonly GetSupplierById _getSupplierById;
    private readonly CreateSupplier _createSupplier;
    private readonly DeactivateSupplier _deactivateSupplier;

    public SuppliersController(
        ListSuppliers listSuppliers,
        GetSupplierById getSupplierById,
        CreateSupplier createSupplier,
        DeactivateSupplier deactivateSupplier)
    {
        _listSuppliers = listSuppliers;
        _getSupplierById = getSupplierById;
        _createSupplier = createSupplier;
        _deactivateSupplier = deactivateSupplier;
    }

    [HttpGet]
    public async Task<ActionResult> GetSuppliers()
    {
        var suppliers = await _listSuppliers.ExecuteAsync();

        return Ok(suppliers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetSupplier([FromRoute] string id)
    {
        if (!Guid.TryParse(id, out _))
        {
            return BadRequest("Invalid supplier id");
        }

        var supplier = await _getSupplierById.ExecuteAsync(id);

        if (supplier == null)
        {
            return NotFound("Supplier not found");
        }

        return Ok(supplier);
    }

    [HttpPost]
    public async Task<ActionResult> AddSupplier([FromBody] CreateSupplierRequest request)
    {
        try
        {
            var supplier = await _createSupplier.ExecuteAsync(
                request.Name,
                request.Country,
                request.ContactEmail,
                request.PhoneNumber
            );

            return CreatedAtAction(nameof(GetSupplier), new { id = supplier.Id }, supplier);
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
        {
            return BadRequest("Invalid supplier id");
        }

        var supplier = await _deactivateSupplier.ExecuteAsync(id);

        if (supplier == null)
        {
            return NotFound("Supplier not found");
        }

        return Ok(supplier);
    }
}