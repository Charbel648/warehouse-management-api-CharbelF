using Microsoft.AspNetCore.Mvc;
using warehouse_management.Contracts;
using warehouse_management.Models;
using warehouse_management.Services;

namespace warehouse_management.Controllers;

[ApiController]
[Route("api/suppliers")]
public class SuppliersController : ControllerBase
{
    private readonly SupplierService _supplierService;

    public SuppliersController(SupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    //this endpoint let you get all the suppliers
    [HttpGet]
    public async Task<ActionResult> GetSuppliers()
    {
        List<Supplier> suppliers = await _supplierService.GetAllSuppliers();

        return Ok(suppliers);
    }

    //this endpoint let you get the supplier by id
    [HttpGet("{id}")]
    public async Task<ActionResult> GetSupplierById([FromRoute] string id)
    {
        if (!Guid.TryParse(id, out _))
        {
            return BadRequest("Invalid supplier id");
        }

        Supplier? supplier = await _supplierService.GetSupplierById(id);

        if (supplier == null)
        {
            return NotFound("Supplier not found");
        }

        return Ok(supplier);
    }

    //this endpoint let you create a new supplier
    [HttpPost]
    public async Task<ActionResult> CreateSupplier([FromBody] CreateSupplierRequest request)
    {
        Supplier supplier = await _supplierService.CreateSupplier(request);

        return CreatedAtAction(nameof(GetSupplierById), new { id = supplier.Id }, supplier);
    }

    //this endpoint let you delete any supplier by id
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeactivateSupplier([FromRoute] string id)
    {
        if (!Guid.TryParse(id, out _))
        {
            return BadRequest("Invalid supplier id");
        }

        Supplier? supplier = await _supplierService.DeactivateSupplier(id);

        if (supplier == null)
        {
            return NotFound("Supplier not found");
        }

        return Ok(supplier);
    }
}