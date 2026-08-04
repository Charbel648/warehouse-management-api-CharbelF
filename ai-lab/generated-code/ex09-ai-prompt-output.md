# Exercise 09 Raw AI Output Simulation — Prompt Engineering

## Weak Prompt Used

Create endpoint for products.

## Typical AI Output From Weak Prompt

The weak prompt usually produces a generic endpoint such as:

```csharp
[HttpGet("products")]
public IActionResult GetProducts()
{
    var products = _context.Products.ToList();
    return Ok(products);
}
```

## Problems in Weak Output

- It may inject DbContext directly into the controller.
- It does not mention authorization.
- It does not mention DTOs.
- It does not mention validation.
- It does not mention unit tests.
- It does not mention integration tests.
- It does not mention Swagger documentation.
- It may return domain entities directly.
- It may ignore the existing Clean Architecture structure.
- It may ignore MediatR/CQRS.
- It may invent missing service classes.
- It does not clarify the exact route, response shape, or business rules.

---

## Strong Prompt Used

Create ASP.NET Core endpoint for warehouse products using controller-service pattern, DTO validation, unit tests, integration tests, and Swagger summary.

## Typical AI Output From Strong Prompt

The strong prompt usually produces a more complete backend feature including:

- controller route
- request DTO
- response DTO
- validation
- service method
- unit tests
- integration tests
- Swagger summary

Example style:

```csharp
[HttpPost]
[ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
public async Task<ActionResult<ProductResponse>> CreateProduct(CreateProductRequest request)
{
    var response = await _productService.CreateProductAsync(request);
    return CreatedAtAction(nameof(GetProduct), new { id = response.Id }, response);
}
```

## Problems in Strong Output

The strong prompt is better than the weak prompt, but it still has one problem for this project.

It says controller-service pattern, while the current Warehouse API uses:

```text
Controller
→ IMediator
→ Command / Query Handler
→ Repository Interface
→ Infrastructure Repository
```

So the AI may generate a ProductService that does not exist in the current architecture.

## Human Correction

For this codebase, the better strong prompt should be:

Create an ASP.NET Core endpoint for warehouse products using the existing Clean Architecture and MediatR/CQRS pattern. The controller must call IMediator only. Add DTO validation, command/query handler, repository boundary if needed, unit tests, integration tests, and Swagger summary. Do not inject DbContext into controllers. Do not invent ProductService if the project already uses handlers.

This corrected prompt fits the actual project better.
