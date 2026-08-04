# Exercise 05 Raw AI Output — Backend Refactoring

## AI Suggested Refactor

The AI suggested:
- Add AsNoTracking to read-only EF Core queries.
- Keep async/await.
- Avoid .Result and .Wait().
- Keep controller thin.
- Avoid adding ProductService because the project uses MediatR/CQRS.
- Keep repository interface unchanged.
- Keep command/update queries tracked where mutation may happen.

## Human Review

Accepted:
- Add AsNoTracking to GetAllAsync because it is read-only.
- Add AsNoTracking to SearchAsync because it is read-only.
- Add AsNoTracking to GetExpiredOrExpiringProductsAsync because it returns read-only data.
- Keep GetByIdAsync tracked because update flows retrieve the product and then mutate it in command handlers.
- Keep AddAsync and UpdateAsync unchanged.
- Keep routes and handlers unchanged.

Rejected:
- Creating a new ProductService.
- Moving EF Core queries into ProductsController.
- Adding Include statements without a business need, because that may increase query cost.
- Using synchronous wrappers such as .Result or .Wait().
- Returning IQueryable outside the repository boundary.

Performance Guardrail:
The accepted refactor does not introduce N+1 queries because it does not add per-row database calls or lazy-loaded navigation access.
