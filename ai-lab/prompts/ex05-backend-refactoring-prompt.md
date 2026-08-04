# Exercise 05 Prompt History — AI Backend Refactoring

## Prompt sent to AI

You are assisting in an ASP.NET Core Warehouse Management API.

The lab asks to refactor ProductService.cs, but this project does not use ProductService.
The current architecture is Clean Architecture with MediatR/CQRS:

ProductsController
→ IMediator
→ Application Handler
→ IProductRepository
→ ProductRepository
→ WarehouseDbContext

Refactor the legacy product backend logic in ProductRepository.cs to improve:
- readability
- safe separation of concerns
- query performance
- maintainability

Current ProductRepository responsibilities:
- Get all products
- Get product by id
- Search products
- Add product
- Update product
- Check duplicate SKU
- Get expired or expiring products

Rules:
- Do not create a ProductService because the project does not use that pattern.
- Do not inject DbContext into controllers.
- Do not change routes.
- Do not change handler contracts.
- Do not change business behavior.
- Do not introduce N+1 queries.
- Do not use .Result or .Wait().
- Keep async/await.
- Use EF Core LINQ safely.
- Use AsNoTracking only on read-only queries.
- Do not use AsNoTracking on GetByIdAsync if the result may be modified later by command handlers.
- Verify tests still pass after refactoring.
