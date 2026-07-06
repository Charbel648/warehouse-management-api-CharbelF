using Microsoft.AspNetCore.Mvc;
using warehouse_management.Contracts;
using warehouse_management.Data;
using warehouse_management.Models;
using System.Globalization;

namespace warehouse_management.Controllers;



[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly ILogger<ProductsController> _logger;
    private readonly IWebHostEnvironment _environment;

    public ProductsController(
        ILogger<ProductsController> logger,
        IWebHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }
    //this endpoint let you get all the products
    [HttpGet()]
    public async Task<ActionResult> GetProducts([FromQuery] bool onlyAvailable = false)
    {
        IEnumerable<Products> products = FakeWarehouseStore.DummyProducts;

        if (onlyAvailable)
        {
            products = products.Where(p => p.QuantityInStock > 0 && p.IsArchived == false);
        }

        var result = await Task.FromResult(
            products
                .OrderByDescending(p => p.Created_at)
                .ToList()
        );

        return Ok(result);
    }
    
    //this endpoint let you get the product by its id
    [HttpGet("{id}")]
    public async Task<ActionResult> GetProduct([FromRoute] string id)
    {
        if (!Guid.TryParse(id, out _))
        {
            return BadRequest("Invalid product id");
        }

        Products? product = await Task.FromResult(
            FakeWarehouseStore.DummyProducts.FirstOrDefault(p => p.Id == id)
        );

        if (product == null)
        {
            return NotFound("Product not found");
        }

        return Ok(product);
    }

    // this endpoint let you get the status of the product by its id (metel getproduct by id bas to check the status only yaane if isArchived or not whith the id and name)
    [HttpGet("{id}/status")]
    public async Task<ActionResult> GetProductStatus([FromRoute] string id)
    {
        Products? product = await Task.FromResult(
            FakeWarehouseStore.DummyProducts.FirstOrDefault(p => p.Id == id)
        );

        if (product == null)
        {
            return NotFound("Product not found");
        }

        return Ok(new
        {
            product.Id,
            product.Name,
            product.IsArchived
        });
    }
    
    //this endpoint let you add a product to the list of products in the warehouse
    [HttpPost()]
    public async Task<ActionResult> AddProduct([FromBody] CreateProductRequest request)
    {
        bool skuExists = FakeWarehouseStore.DummyProducts.Any(p =>
            p.SKU.Equals(request.SKU, StringComparison.OrdinalIgnoreCase));

        if (skuExists)
        {
            return BadRequest("A product with the same SKU already exists");
        }
        
        Products product = new Products
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Name,
            SKU = request.SKU,
            Description = request.Description,
            Price = request.Price,
            QuantityInStock = request.QuantityInStock,
            SupplierName = request.SupplierName,
            ExpiryDate = request.ExpiryDate,
            IsArchived = false,
            Created_at = DateTime.UtcNow,
            Last_Updated_at = DateTime.UtcNow
        };

        FakeWarehouseStore.DummyProducts.Add(product);

        await Task.CompletedTask;

        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    //this endpoint let you update the quantityinstock of the product when you provide the id 
    //bel lab mahtout nesta3moul post bas i think enno put is more approriate
    [HttpPut("{id}/quantity")]
    public async Task<ActionResult> UpdateProductQuantity(
        [FromRoute] string id,
        [FromBody] UpdateProductQuantityRequest request)
    {
        if (!Guid.TryParse(id, out _))
        {
            return BadRequest("Invalid product id");
        }

        if (request.QuantityInStock < 0)
        {
            return BadRequest("Quantity cannot be negative");
        }
        
        Products? product = await Task.FromResult(
            FakeWarehouseStore.DummyProducts.FirstOrDefault(p => p.Id == id)
        );

        if (product == null)
        {
            return NotFound("Product not found");
        }

        product.QuantityInStock = request.QuantityInStock;
        product.Last_Updated_at = DateTime.UtcNow;

        return Ok(product);
    }
    
    //this endpoint let you update the proce of the product if you provide it with the id
    //bel lab mahtout nesta3moul post bas i think enno put is more approriate
    [HttpPut("{id}/price")]
    public async Task<ActionResult> UpdateProductPrice(
        [FromRoute] string id,
        [FromBody] UpdateProductPriceRequest request)
    {
        if (!Guid.TryParse(id, out _))
        {
            return BadRequest("Invalid product id");
        }

        if (request.Price <= 0)
        {
            return BadRequest("Price must be greater than 0");
        }
        
        Products? product = await Task.FromResult(
            FakeWarehouseStore.DummyProducts.FirstOrDefault(p => p.Id == id)
        );

        if (product == null)
        {
            return NotFound("Product not found");
        }

        product.Price = request.Price;
        product.Last_Updated_at = DateTime.UtcNow;

        return Ok(product);
    }

    //this endpoint let you delete the product by id
    [HttpDelete("{id}")]
    public async Task<ActionResult> ArchiveProduct([FromRoute] string id)
    {
        if (!Guid.TryParse(id, out _))
        {
            return BadRequest("Invalid product id");
        }
        
        Products? product = await Task.FromResult(
            FakeWarehouseStore.DummyProducts.FirstOrDefault(p => p.Id == id)
        );

        if (product == null)
        {
            return NotFound("Product not found");
        }

        product.IsArchived = true;
        product.Last_Updated_at = DateTime.UtcNow;

        return Ok(product);
    }
    
    //this endpoint let you search the product if you provide it with the name of product or the supplier's name
    [HttpGet("search")]
    public async Task<ActionResult> SearchProducts(
        [FromQuery] string? name,
        [FromQuery] string? supplier)
    {
        if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(supplier))
        {
            return BadRequest("You must provide name or supplier");
        }

        IEnumerable<Products> products = FakeWarehouseStore.DummyProducts;

        if (!string.IsNullOrWhiteSpace(name))
        {
            products = products.Where(p =>
                p.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(supplier))
        {
            products = products.Where(p =>
                p.SupplierName.Contains(supplier, StringComparison.OrdinalIgnoreCase));
        }

        var result = await Task.FromResult(products.ToList());

        return Ok(result);
    }
    
    //this endpoint let you upload a product's image and save it in the uploads foler 
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

        Products? product = await Task.FromResult(
            FakeWarehouseStore.DummyProducts.FirstOrDefault(p => p.Id == id)
        );

        if (product == null)
        {
            return NotFound("Product not found");
        }

        IFormFile image = request.Image;

        if (image == null || image.Length == 0)
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

        ProductImage productImage = new ProductImage
        {
            ProductId = product.Id,
            FileName = fileName,
            FilePath = $"/uploads/{fileName}"
        };

        FakeWarehouseStore.ProductImages.Add(productImage);

        product.Last_Updated_at = DateTime.UtcNow;

        return Ok(productImage);
    }
    
    //this endpoint gives you the current time in a language that you select
    [HttpGet("server-time")]
    public async Task<ActionResult> GetServerTime(
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

        var result = new
        {
            Language = selectedLanguage,
            ServerTime = DateTime.Now.ToString("F", culture)
        };

        return await Task.FromResult(Ok(result));
    }
    
    //this endpoint gives the ability to assign a supplier to a product (Creates a Product-Supplier Link)
    [HttpPost("{id}/assign-supplier/{supplierId}")]
    public async Task<ActionResult> AssignSupplierToProduct(
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

        Products? product = await Task.FromResult(
            FakeWarehouseStore.DummyProducts.FirstOrDefault(p => p.Id == id)
        );

        if (product == null)
        {
            return NotFound("Product not found");
        }

        Supplier? supplier = await Task.FromResult(
            FakeWarehouseStore.DummySuppliers.FirstOrDefault(s => s.Id == supplierId)
        );

        if (supplier == null)
        {
            return NotFound("Supplier not found");
        }

        if (product.IsArchived)
        {
            return BadRequest("Archived products cannot be assigned to a supplier");
        }

        if (!supplier.IsActive)
        {
            return BadRequest("Inactive suppliers cannot be assigned");
        }

        product.SupplierId = supplier.Id;
        product.SupplierName = supplier.Name;
        product.Last_Updated_at = DateTime.UtcNow;

        return Ok(product);
    }
    
}