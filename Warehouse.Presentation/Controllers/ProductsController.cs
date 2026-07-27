using System.Globalization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Application.Files.Commands.UploadProductImage;
using Warehouse.Application.Products.Commands.ArchiveProduct;
using Warehouse.Application.Products.Commands.AssignSupplierToProduct;
using Warehouse.Application.Products.Commands.CreateProduct;
using Warehouse.Application.Products.Commands.UpdateProductPrice;
using Warehouse.Application.Products.Commands.UpdateProductQuantity;
using Warehouse.Application.Products.Queries.GetProductById;
using Warehouse.Application.Products.Queries.ListProducts;
using Warehouse.Application.Products.Queries.SearchProducts;
using Warehouse.Domain.Exceptions;
using Warehouse.Presentation.Contracts;
using Warehouse.Presentation.Security;

namespace Warehouse.Presentation.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Policy = WarehousePolicies.WarehouseReader)]
    [HttpGet]
    public async Task<ActionResult> GetProducts(
        [FromQuery] bool onlyAvailable = false,
        CancellationToken cancellationToken = default)
    {
        var products = await _mediator.Send(new ListProductsQuery
        {
            OnlyAvailable = onlyAvailable
        }, cancellationToken);

        return Ok(products);
    }

    [Authorize(Policy = WarehousePolicies.WarehouseReader)]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetProduct(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        string productId = id.ToString();

        var product = await _mediator.Send(new GetProductByIdQuery
        {
            ProductId = productId
        }, cancellationToken);

        if (product == null)
            throw new NotFoundException("Product", productId);

        return Ok(product);
    }

    [Authorize(Policy = WarehousePolicies.WarehouseReader)]
    [HttpGet("search")]
    public async Task<ActionResult> SearchProducts(
        [FromQuery] string? name,
        [FromQuery] string? supplier,
        CancellationToken cancellationToken)
    {
        var products = await _mediator.Send(new SearchProductsQuery
        {
            Name = name,
            Supplier = supplier
        }, cancellationToken);

        return Ok(products);
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

    [Authorize(Policy = WarehousePolicies.WarehouseAdmin)]
    [HttpPost]
    public async Task<ActionResult> AddProduct(
        [FromBody] CreateProductRequest request,
        CancellationToken cancellationToken)
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
        }, cancellationToken);

        return CreatedAtAction(nameof(GetProduct), new { id = response.Id }, response);
    }

    [Authorize(Policy = WarehousePolicies.WarehouseAdmin)]
    [HttpPut("{id:guid}/quantity")]
    public async Task<ActionResult> UpdateQuantity(
        [FromRoute] Guid id,
        [FromBody] UpdateProductQuantityRequest request,
        CancellationToken cancellationToken)
    {
        string productId = id.ToString();

        var product = await _mediator.Send(new UpdateProductQuantityCommand
        {
            ProductId = productId,
            QuantityInStock = request.QuantityInStock
        }, cancellationToken);

        if (product == null)
            throw new NotFoundException("Product", productId);

        return Ok(product);
    }

    [Authorize(Policy = WarehousePolicies.WarehouseAdmin)]
    [HttpPut("{id:guid}/price")]
    public async Task<ActionResult> UpdatePrice(
        [FromRoute] Guid id,
        [FromBody] UpdateProductPriceRequest request,
        CancellationToken cancellationToken)
    {
        string productId = id.ToString();

        var product = await _mediator.Send(new UpdateProductPriceCommand
        {
            ProductId = productId,
            Price = request.Price
        }, cancellationToken);

        if (product == null)
            throw new NotFoundException("Product", productId);

        return Ok(product);
    }

    [Authorize(Policy = WarehousePolicies.WarehouseAdmin)]
    [HttpPost("{id:guid}/image")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult> UploadProductImage(
        [FromRoute] Guid id,
        [FromForm] UploadProductImageRequest request,
        CancellationToken cancellationToken)
    {
        await using Stream content = request.Image.OpenReadStream();

        var response = await _mediator.Send(new UploadProductImageCommand
        {
            ProductId = id.ToString(),
            Content = content,
            FileName = request.Image.FileName,
            ContentType = request.Image.ContentType,
            SizeInBytes = request.Image.Length
        }, cancellationToken);

        return Ok(response);
    }

    [Authorize(Policy = WarehousePolicies.WarehouseAdmin)]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteProduct(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        string productId = id.ToString();

        var product = await _mediator.Send(new ArchiveProductCommand
        {
            ProductId = productId
        }, cancellationToken);

        if (product == null)
            throw new NotFoundException("Product", productId);

        return Ok(product);
    }

    [Authorize(Policy = WarehousePolicies.WarehouseAdmin)]
    [HttpPost("{id:guid}/assign-supplier/{supplierId:guid}")]
    public async Task<ActionResult> AssignSupplier(
        [FromRoute] Guid id,
        [FromRoute] Guid supplierId,
        CancellationToken cancellationToken)
    {
        string productId = id.ToString();
        string supplierIdValue = supplierId.ToString();

        var response = await _mediator.Send(new AssignSupplierToProductCommand
        {
            ProductId = productId,
            SupplierId = supplierIdValue
        }, cancellationToken);

        if (response == null)
            throw new NotFoundException("Product", productId);

        return Ok(response);
    }
}

