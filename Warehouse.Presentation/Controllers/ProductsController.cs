using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Application.Products.Commands;
using Warehouse.Application.Products.Queries;
using Warehouse.Presentation.Contracts;

namespace Warehouse.Presentation.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly ListProducts _listProducts;
    private readonly GetProductById _getProductById;
    private readonly SearchProducts _searchProducts;
    private readonly CreateProduct _createProduct;
    private readonly UpdateProductQuantity _updateProductQuantity;
    private readonly UpdateProductPrice _updateProductPrice;
    private readonly ArchiveProduct _archiveProduct;
    private readonly AssignSupplierToProduct _assignSupplierToProduct;
    private readonly AddProductImage _addProductImage;
    private readonly IWebHostEnvironment _environment;

    public ProductsController(
        ListProducts listProducts,
        GetProductById getProductById,
        SearchProducts searchProducts,
        CreateProduct createProduct,
        UpdateProductQuantity updateProductQuantity,
        UpdateProductPrice updateProductPrice,
        ArchiveProduct archiveProduct,
        AssignSupplierToProduct assignSupplierToProduct,
        AddProductImage addProductImage,
        IWebHostEnvironment environment)
    {
        _listProducts = listProducts;
        _getProductById = getProductById;
        _searchProducts = searchProducts;
        _createProduct = createProduct;
        _updateProductQuantity = updateProductQuantity;
        _updateProductPrice = updateProductPrice;
        _archiveProduct = archiveProduct;
        _assignSupplierToProduct = assignSupplierToProduct;
        _addProductImage = addProductImage;
        _environment = environment;
    }

    [HttpGet]
    public async Task<ActionResult> GetProducts([FromQuery] bool onlyAvailable = false)
    {
        var products = await _listProducts.ExecuteAsync(onlyAvailable);

        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetProduct([FromRoute] string id)
    {
        if (!Guid.TryParse(id, out _))
        {
            return BadRequest("Invalid product id");
        }

        var product = await _getProductById.ExecuteAsync(id);

        if (product == null)
        {
            return NotFound("Product not found");
        }

        return Ok(product);
    }

    [HttpGet("search")]
    public async Task<ActionResult> SearchProducts(
        [FromQuery] string? name,
        [FromQuery] string? supplier)
    {
        try
        {
            var products = await _searchProducts.ExecuteAsync(name, supplier);

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
            {
                selectedLanguage = language;
            }
        }

        CultureInfo culture = new CultureInfo(selectedLanguage);

        return Ok(new
        {
            Language = selectedLanguage,
            ServerTime = DateTime.Now.ToString("F", culture)
        });
    }

    [HttpPost]
    public async Task<ActionResult> AddProduct([FromBody] CreateProductRequest request)
    {
        try
        {
            var product = await _createProduct.ExecuteAsync(
                request.Name,
                request.SKU,
                request.Description,
                request.Price,
                request.QuantityInStock,
                request.SupplierName,
                request.ExpiryDate
            );

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
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
        {
            return BadRequest("Invalid product id");
        }

        try
        {
            var product = await _updateProductQuantity.ExecuteAsync(
                id,
                request.QuantityInStock
            );

            if (product == null)
            {
                return NotFound("Product not found");
            }

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
        {
            return BadRequest("Invalid product id");
        }

        try
        {
            var product = await _updateProductPrice.ExecuteAsync(
                id,
                request.Price
            );

            if (product == null)
            {
                return NotFound("Product not found");
            }

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
        {
            return BadRequest("Invalid product id");
        }

        var product = await _getProductById.ExecuteAsync(id);

        if (product == null)
        {
            return NotFound("Product not found");
        }

        IFormFile image = request.Image;

        if (image.Length == 0)
        {
            return BadRequest("Image is required");
        }

        long maxSize = 2 * 1024 * 1024;

        if (image.Length > maxSize)
        {
            return BadRequest("Image size cannot be more than 2 MB");
        }

        string extension = Path.GetExtension(image.FileName).ToLower();

        if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
        {
            return BadRequest("Only JPG and PNG images are allowed");
        }

        string webRootPath = _environment.WebRootPath
                             ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

        string uploadsFolder = Path.Combine(webRootPath, "uploads");

        Directory.CreateDirectory(uploadsFolder);

        string fileName = $"{Guid.NewGuid()}{extension}";
        string fullPath = Path.Combine(uploadsFolder, fileName);

        using FileStream stream = new FileStream(fullPath, FileMode.Create);
        await image.CopyToAsync(stream);

        var productImage = await _addProductImage.ExecuteAsync(
            id,
            fileName,
            $"/uploads/{fileName}"
        );

        return Ok(productImage);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProduct([FromRoute] string id)
    {
        if (!Guid.TryParse(id, out _))
        {
            return BadRequest("Invalid product id");
        }

        var product = await _archiveProduct.ExecuteAsync(id);

        if (product == null)
        {
            return NotFound("Product not found");
        }

        return Ok(product);
    }

    [HttpPost("{id}/assign-supplier/{supplierId}")]
    public async Task<ActionResult> AssignSupplier(
        [FromRoute] string id,
        [FromRoute] string supplierId)
    {
        if (!Guid.TryParse(id, out _))
        {
            return BadRequest("Invalid product id");
        }

        if (!Guid.TryParse(supplierId, out _))
        {
            return BadRequest("Invalid supplier id");
        }

        try
        {
            var product = await _assignSupplierToProduct.ExecuteAsync(id, supplierId);

            if (product == null)
            {
                return NotFound("Product not found");
            }

            return Ok(product);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}