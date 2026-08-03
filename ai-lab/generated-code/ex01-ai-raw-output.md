# Exercise 01 Raw AI Output — Before Manual Refactoring

The AI initially suggested adding a ProductService method and calling it from the controller.

Rejected parts:
- Creating ProductService would not match the existing codebase because the project uses MediatR/CQRS handlers.
- Injecting repository logic directly into the controller would break the thin-controller pattern.
- Returning domain entities directly would weaken DTO separation.

Manual architecture correction:
Use the existing project flow:

ProductsController
→ IMediator
→ GetExpiringSoonProductsQuery
→ GetExpiringSoonProductsHandler
→ IProductRepository
→ ProductRepository EF Core implementation

Final accepted design:
- Add GET /api/products/expiring-soon to ProductsController.
- Add GetExpiringSoonProductsQuery.
- Add GetExpiringSoonProductsHandler.
- Add ExpiringSoonProductDto.
- Add IProductRepository.GetExpiringSoonAsync(...).
- Add ProductRepository.GetExpiringSoonAsync(...).
- Add isolated unit tests using Moq.
