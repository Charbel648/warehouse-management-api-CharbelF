# Exercise 05 — AI Backend Refactoring Review

## Refactored File

Warehouse.Infrastructure/Repositories/ProductRepository.cs

## Why ProductRepository Was Selected

The lab refers to ProductService.cs, but this project does not use ProductService.
The actual architecture uses MediatR/CQRS handlers and repository interfaces.

Creating a new ProductService would violate the existing structure and previous supervisor feedback.
Therefore, the refactoring target was ProductRepository.cs, which is the existing product data-access implementation.

## Refactor Summary

Applied safe read-query optimization:

- Added AsNoTracking() to GetAllAsync.
- Added AsNoTracking() to SearchAsync.
- Added AsNoTracking() to GetExpiredOrExpiringProductsAsync.
- Did not add AsNoTracking() to GetByIdAsync because command handlers use this method before mutating product state.

## Architecture Verification

The refactor preserved:

- Controller → IMediator flow
- Application handler boundaries
- Repository interface usage
- Infrastructure-only EF Core access
- Existing routes
- Existing DTO/response behavior

No DbContext was introduced into the Presentation layer.

## Performance Guardrail

### N+1 Query Check

The refactor did not introduce N+1 queries.

Reason:
- No per-product database calls were added.
- No lazy-loaded navigation access was added.
- No unnecessary Include statements were added.
- Filtering remains inside EF Core LINQ and is translated to SQL.

### Blocking Async Check

The refactor did not introduce blocking async wrappers.

Checked for:

- .Result
- .Wait()

Result:

No usage added.

## Human Corrections to AI Output

The AI suggested applying AsNoTracking broadly. Human review rejected applying it to GetByIdAsync because that method is used by update/archive/assign-supplier command handlers. Keeping it tracked avoids subtle update behavior changes.

The AI also suggested adding Include for Supplier in some queries. This was rejected because current DTOs use SupplierName and do not need the navigation object. Adding Include could increase query cost without benefit.

## Regression Verification

Commands:

dotnet test .\tests\Warehouse.Api.UnitTests\Warehouse.Api.UnitTests.csproj
dotnet test .\tests\Warehouse.Api.IntegrationTests\Warehouse.Api.IntegrationTests.csproj
dotnet test .\warehouse_management.sln

Expected result:

All tests pass.

## Final Decision

The refactor is accepted because it improves read-query performance and readability while preserving routes, logic, tests, and architecture.
