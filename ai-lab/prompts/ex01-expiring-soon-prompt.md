# Exercise 01 Prompt History — Expiring Soon Products Endpoint

## Prompt sent to AI

You are assisting in an ASP.NET Core Warehouse Management API that already uses Clean Architecture with:
- Presentation controllers
- MediatR commands/queries
- Application handlers
- Domain models
- Repository interfaces
- Infrastructure repository implementations
- Unit tests with Moq and FluentAssertions

Generate a new endpoint:

GET /api/products/expiring-soon

Business rule:
Return products scheduled to expire within the next 30 calendar days.
Do not include archived products.
Do not include products already expired before today.

Architecture requirements:
- Keep controller thin.
- Controller must call IMediator only.
- Do not inject DbContext into controller.
- Do not create a random ProductService if the project already uses MediatR/CQRS.
- Add application query and handler.
- Add DTO/response shape.
- Add repository interface method.
- Add infrastructure repository implementation using EF Core LINQ.
- Add unit tests with mocked repository.
- Use async/await.
- Do not use .Result or .Wait().
- Keep code testable and mockable.
