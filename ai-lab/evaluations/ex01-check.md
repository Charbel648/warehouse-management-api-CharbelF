# Exercise 01 — AI-Assisted API Feature Development Evaluation

## Feature

New endpoint:

GET /api/products/expiring-soon

Business rule:

Return all non-archived products whose expiry date is between today and the next 30 calendar days.

## AI Prompt Summary

The AI was asked to generate an ASP.NET Core backend feature for the warehouse-management-api project using the existing Clean Architecture structure.

The prompt required:
- controller action and route binding
- application-level query logic
- DTO response shape
- repository boundary
- unit tests with mocked dependencies
- no direct DbContext usage inside controllers
- no new ProductService if the codebase already uses MediatR/CQRS
- async/await only
- no .Result or .Wait()

## AI Output Review

The AI initially suggested a ProductService-style approach. This was reviewed and manually corrected because the current project uses MediatR query/command handlers.

Accepted final design:

ProductsController
→ IMediator
→ GetExpiringSoonProductsQuery
→ GetExpiringSoonProductsHandler
→ IProductRepository
→ ProductRepository

## Implemented Files

- Warehouse.Application/Products/Queries/GetExpiringSoonProducts/ExpiringSoonProductDto.cs
- Warehouse.Application/Products/Queries/GetExpiringSoonProducts/GetExpiringSoonProductsQuery.cs
- Warehouse.Application/Products/Queries/GetExpiringSoonProducts/GetExpiringSoonProductsHandler.cs
- Warehouse.Domain/Repositories/IProductRepository.cs
- Warehouse.Infrastructure/Repositories/ProductRepository.cs
- Warehouse.Presentation/Controllers/ProductsController.cs
- tests/Warehouse.Api.UnitTests/Products/GetExpiringSoonProductsHandlerTests.cs
- tests/Warehouse.Api.UnitTests/Builders/ProductBuilder.cs

## Structural Evaluation Table

| Area | Check Requirement | Acceptable? (Y/N) | Notes |
|---|---|---:|---|
| Code Quality | Does it follow C# naming conventions and clean programming principles? | Y | Classes, methods, DTOs, and query handler names follow existing C# naming conventions. Logic is simple and readable. |
| Architecture | Does it strictly match the existing design patterns of the codebase? | Y | The implementation follows the existing Controller → IMediator → Query Handler → Repository pattern. No DbContext was injected into the controller. No unnecessary ProductService was created. |
| Validation | Is parameter or model state boundary validation properly executed? | Y | The endpoint has no request body or user-provided filter parameters. The business boundary is enforced inside the handler and repository: current date through current date + 30 days. Archived and already-expired products are excluded. |
| Testability | Is the generated code easily mockable without hacking dependencies? | Y | The handler depends only on IProductRepository, so unit tests can mock the repository using Moq. No infrastructure dependency is required inside the unit test. |

## Manual Corrections Applied

1. Rejected AI suggestion to create a new ProductService because it would not match the current CQRS/MediatR architecture.
2. Added a dedicated query and handler instead of placing business logic in the controller.
3. Ensured repository filtering excludes archived products.
4. Fixed the existing repository method to exclude already-expired products by checking:
   - ExpiryDate >= currentDate
   - ExpiryDate <= expiringLimitDate
5. Added a DTO instead of exposing unnecessary domain internals.
6. Added unit tests around the handler and repository mock boundary.

## Verification

Commands executed:

dotnet test .\tests\Warehouse.Api.UnitTests\Warehouse.Api.UnitTests.csproj
dotnet test .\warehouse_management.sln

Result:

All tests passed.

## Final Decision

Exercise 01 is accepted after human review and manual refactoring because it preserves the existing backend architecture, keeps the controller thin, avoids infrastructure leakage, and includes isolated unit tests.
