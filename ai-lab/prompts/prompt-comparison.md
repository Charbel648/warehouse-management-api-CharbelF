# Exercise 09 — Prompt Comparison

## Goal

Compare two AI prompts and document how prompt quality affects backend code generation.

The lab requires comparing:

Weak prompt:

```text
Create endpoint for products.
```

Strong prompt:

```text
Create ASP.NET Core endpoint for warehouse products using controller-service pattern, DTO validation, unit tests, integration tests, and Swagger summary.
```

The comparison must document:

- differences
- output quality
- missing details
- code output variances
- error rates
- omissions

---

# 1. Weak Prompt Review

## Weak Prompt

```text
Create endpoint for products.
```

## Expected AI Behavior

This prompt is too short and vague.

It does not tell the AI:

- what framework to use
- what route to create
- what HTTP method to use
- whether this is a read endpoint or write endpoint
- what DTOs are required
- what validation rules are needed
- whether authorization is required
- what architecture the project uses
- whether to use services, handlers, or repositories
- whether to write tests
- whether to update Swagger
- what response model should look like
- what errors should be handled

## Typical Output Quality

Low.

The AI may generate a generic controller that works in isolation but does not fit the real project.

Example weak output risk:

```csharp
[HttpGet("products")]
public IActionResult GetProducts()
{
    var products = _context.Products.ToList();
    return Ok(products);
}
```

## Main Problems

| Problem | Explanation |
|---|---|
| Architecture mismatch | AI may inject DbContext directly into the controller |
| Missing DTOs | AI may return domain entities directly |
| Missing validation | AI does not know which input rules to enforce |
| Missing authorization | AI may create an unprotected endpoint |
| Missing tests | AI was not asked to create unit or integration tests |
| Missing Swagger details | AI was not asked to document response codes |
| Missing business rules | AI does not know product-specific constraints |
| Possible synchronous code | AI may generate non-async or blocking code |
| No repository boundary | AI may skip repository interfaces |
| No project awareness | AI may ignore MediatR/CQRS |

## Error Rate Estimate

High.

Estimated risk level:

```text
70% to 90%
```

Reason:

The prompt does not provide enough architectural or business context, so most generated output will require heavy manual correction.

---

# 2. Strong Prompt Review

## Strong Prompt

```text
Create ASP.NET Core endpoint for warehouse products using controller-service pattern, DTO validation, unit tests, integration tests, and Swagger summary.
```

## Expected AI Behavior

This prompt is much better because it gives the AI more backend engineering requirements.

It clearly asks for:

- ASP.NET Core
- warehouse products endpoint
- DTO validation
- unit tests
- integration tests
- Swagger summary

## Typical Output Quality

Medium to high.

The AI is more likely to generate a full backend feature instead of only a controller method.

## Improvements Over Weak Prompt

| Area | Weak Prompt | Strong Prompt |
|---|---|---|
| Framework | Not specified | ASP.NET Core specified |
| Domain | Generic products | Warehouse products |
| Validation | Missing | DTO validation requested |
| Testing | Missing | Unit and integration tests requested |
| Documentation | Missing | Swagger summary requested |
| Architecture | Not specified | Controller-service pattern specified |
| Output completeness | Low | Higher |
| Review effort | High | Medium |

## Remaining Problem

The strong prompt says:

```text
controller-service pattern
```

But this project does not use a ProductService for product features.

The current Warehouse API uses:

```text
ProductsController
→ IMediator
→ Commands / Queries
→ Handlers
→ IProductRepository
→ ProductRepository
```

So the AI may generate a `ProductService` class that conflicts with the real architecture.

This is better than the weak prompt, but still not perfect for this codebase.

## Error Rate Estimate

Medium.

Estimated risk level:

```text
30% to 50%
```

Reason:

The prompt contains useful engineering details, but the architecture pattern is partially wrong for the existing project.

---

# 3. Best Corrected Prompt for This Project

The best prompt should preserve the real architecture.

## Corrected Strong Prompt

```text
Create an ASP.NET Core endpoint for warehouse products using the existing Clean Architecture and MediatR/CQRS pattern.

Requirements:
- Controller must call IMediator only.
- Do not inject DbContext into the controller.
- Do not create ProductService if the project already uses handlers.
- Add request DTO and response DTO if needed.
- Use DataAnnotations or existing validation style.
- Add an application command/query and handler.
- Use repository interfaces from the Domain layer.
- Keep EF Core inside Infrastructure repositories only.
- Add unit tests with mocked dependencies.
- Add WebApplicationFactory integration tests.
- Assert status codes, headers, response properties, and server-side effects.
- Add Swagger response summary.
- Preserve authorization policies.
- Use async/await only.
- Do not use .Result or .Wait().
```

## Why This Prompt Is Better

This prompt is stronger because it gives the AI:

- exact architecture
- exact dependency direction
- testing expectations
- validation expectations
- authorization expectations
- async safety rules
- anti-hallucination constraints

It reduces the chance that AI generates code that looks correct but violates the project structure.

---

# 4. Output Variance Comparison

| Comparison Area | Weak Prompt Output | Strong Prompt Output | Corrected Prompt Output |
|---|---|---|---|
| Controller | Generic controller method | Controller with service call | Thin controller using IMediator |
| Business logic | Often inside controller | Usually inside service | Inside application handler/domain model |
| Data access | Direct DbContext risk | Service may call repository | Repository interface and infrastructure implementation |
| DTOs | Usually missing | Usually included | Included and aligned with project |
| Validation | Usually missing | Usually included | Included using project style |
| Authorization | Often missing | Sometimes missing | Explicitly required |
| Unit tests | Missing | Included | Included with mocked interfaces |
| Integration tests | Missing | Included | Included using WebApplicationFactory |
| Swagger | Missing | Included | Included with proper route/response info |
| Architecture fit | Poor | Partial | Strong |
| Human correction required | Heavy | Medium | Low |

---

# 5. Omissions Found

## Weak Prompt Omissions

The weak prompt omitted:

- route name
- HTTP method
- request model
- response model
- validation rules
- authorization policy
- architecture pattern
- error handling
- tests
- Swagger
- async requirements
- repository boundary
- database access rules

## Strong Prompt Omissions

The strong prompt omitted:

- current project architecture
- MediatR/CQRS usage
- instruction not to create ProductService
- authorization policy details
- no DbContext in controller rule
- async safety guardrails
- repository layer boundaries
- external infrastructure boundaries

---

# 6. Final Decision

The weak prompt is not acceptable for backend production code generation because it is too vague.

The strong prompt is better, but it still needs project-specific architecture context.

For this Warehouse API, the best prompt is the corrected strong prompt that explicitly tells the AI to use:

```text
Controller
→ IMediator
→ Command / Query Handler
→ Repository Interface
→ Infrastructure Repository
```

This reduces hallucinated architecture, improves testability, and produces code that better matches the existing backend solution.
