using System.Globalization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Application.Products.Commands.AddProductImage;
using Warehouse.Application.Products.Commands.ArchiveProduct;
using Warehouse.Application.Products.Commands.AssignSupplierToProduct;
using Warehouse.Application.Products.Commands.CreateProduct;
using Warehouse.Application.Products.Commands.UpdateProductPrice;
using Warehouse.Application.Products.Commands.UpdateProductQuantity;
using Warehouse.Application.Products.Queries.GetProductById;
using Warehouse.Application.Products.Queries.ListProducts;
using Warehouse.Application.Products.Queries.SearchProducts;
using Warehouse.Presentation.Contracts;

namespace Warehouse.Presentation.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IWebHostEnvironment _environment;

    public ProductsController(
        IMediator mediator,
        IWebHostEnvironment environment)
    {
        _mediator = mediator;
        _environment = environment;
    }

    [HttpGet]
    public async Task<ActionResult> GetProducts([FromQuery] bool onlyAvailable = false)
    {
        var products = await _mediator.Send(new ListProductsQuery
        {
            OnlyAvailable = onlyAvailable
        });

        return Ok(products);
    }

    [HttpGet("search")]
    public async Task<ActionResult> SearchProducts(
        [FromQuery] string? name,
        [FromQuery] string? supplier)
    {
        try
        {
            var products = await _mediator.Send(new SearchProductsQuery
            {
                Name = name,
                Supplier = supplier
            });

            return Ok(products);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("server-time")]
    public ActionResult GetServerTime(
        [FromHeader(Name = "Accept-Language")] string? acceptLanguage)
    {
        string selectedLanguage = "en-US";

        if (!string.IsNullOrWhiteSpace(acceptLanguage))
        {
            string language = acceptLanguage.Split(',')[0].Trim();

            if (language == "en-US" || language == "fr-FR" || language == "ar-LB")
                selectedLanguage = language;
        }

        CultureInfo culture = new CultureInfo(selectedLanguage);

        return Ok(new
        {
            Language = selectedLanguage,
            ServerTime = DateTime.Now.ToString("F", culture)
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetProduct([FromRoute] string id)
    {
        if (!Guid.TryParse(id, out _))
            return BadRequest("Invalid product id");

        var product = await _mediator.Send(new GetProductByIdQuery
        {
            ProductId = id
        });

        if (product == null)
            return NotFound("Product not found");

        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult> AddProduct([FromBody] CreateProductRequest request)
    {
        try
        {
            var response = await _mediator.Send(new CreateProductCommand
            {
                Name = request.Name,
                SKU = request.SKU,
                Description = request.Description,
                Price = request.Price,
                QuantityInStock = request.QuantityInStock,
                SupplierName = request.SupplierName,
                ExpiryDate = request.ExpiryDate
            });

            return CreatedAtAction(nameof(GetProduct), new { id = response.Id }, response);
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

    [HttpPut("{id}/quantity")]
    public async Task<ActionResult> UpdateQuantity(
        [FromRoute] string id,
        [FromBody] UpdateProductQuantityRequest request)
    {
        if (!Guid.TryParse(id, out _))
            return BadRequest("Invalid product id");

        try
        {
            var product = await _mediator.Send(new UpdateProductQuantityCommand
            {
                ProductId = id,
                QuantityInStock = request.QuantityInStock
            });

            if (product == null)
                return NotFound("Product not found");

            return Ok(product);
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

    [HttpPut("{id}/price")]
    public async Task<ActionResult> UpdatePrice(
        [FromRoute] string id,
        [FromBody] UpdateProductPriceRequest request)
    {
        if (!Guid.TryParse(id, out _))
            return BadRequest("Invalid product id");

        try
        {
            var product = await _mediator.Send(new UpdateProductPriceCommand
            {
                ProductId = id,
                Price = request.Price
            });

            if (product == null)
                return NotFound("Product not found");

            return Ok(product);
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

    [HttpPost("{id}/image")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult> UploadProductImage(
        [FromRoute] string id,
        [FromForm] UploadProductImageRequest request)
    {
        if (!Guid.TryParse(id, out _))
            return BadRequest("Invalid product id");

        IFormFile image = request.Image;

        if (image == null || image.Length == 0)
            return BadRequest("Image is required");

        long maxSize = 2 * 1024 * 1024;

        if (image.Length > maxSize)
            return BadRequest("Image size cannot be more than 2 MB");

        string extension = Path.GetExtension(image.FileName).ToLower();

        if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
            return BadRequest("Only JPG and PNG images are allowed");

        string webRootPath = _environment.WebRootPath
                             ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

        string uploadsFolder = Path.Combine(webRootPath, "uploads");

        Directory.CreateDirectory(uploadsFolder);

        string fileName = $"{Guid.NewGuid()}{extension}";
        string fullPath = Path.Combine(uploadsFolder, fileName);

        using FileStream stream = new FileStream(fullPath, FileMode.Create);
        await image.CopyToAsync(stream);

        try
        {
            var response = await _mediator.Send(new AddProductImageCommand
            {
                ProductId = id,
                FileName = fileName,
                FilePath = $"/uploads/{fileName}"
            });

            if (response == null)
                return NotFound("Product not found");

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProduct([FromRoute] string id)
    {
        if (!Guid.TryParse(id, out _))
            return BadRequest("Invalid product id");

        var product = await _mediator.Send(new ArchiveProductCommand
        {
            ProductId = id
        });

        if (product == null)
            return NotFound("Product not found");

        return Ok(product);
    }

    [HttpPost("{id}/assign-supplier/{supplierId}")]
    public async Task<ActionResult> AssignSupplier(
        [FromRoute] string id,
        [FromRoute] string supplierId)
    {
        if (!Guid.TryParse(id, out _))
            return BadRequest("Invalid product id");

        if (!Guid.TryParse(supplierId, out _))
            return BadRequest("Invalid supplier id");

        try
        {
            var response = await _mediator.Send(new AssignSupplierToProductCommand
            {
                ProductId = id,
                SupplierId = supplierId
            });

            if (response == null)
                return NotFound("Product not found");

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
