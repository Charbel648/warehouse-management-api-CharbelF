# Exercise 06 Prompt History — AI API Documentation

## Prompt sent to AI

You are documenting an ASP.NET Core Warehouse Management API.

The project uses:
- Clean Architecture
- ASP.NET Core Controllers
- MediatR/CQRS commands and queries
- Application handlers
- Domain models
- Repository interfaces
- Infrastructure repository implementations
- Firebase JWT authorization policies
- MinIO object storage for files
- RabbitMQ events for warehouse notifications
- Unit and integration tests

Generate clear API documentation for:
- Product endpoints
- Supplier endpoints
- Stock adjustment endpoint
- Inventory dashboard endpoint
- File download endpoint
- Expiring soon products endpoint
- Product image upload
- Supplier document upload

Documentation must include:
- endpoint summary
- HTTP method and route
- authorization requirement
- request body examples
- response examples
- architecture notes
- error behavior

Rules:
- Do not expose secrets.
- Use placeholder tokens only.
- Do not invent endpoints that are not in the project.
- Keep the documentation aligned with the existing controller routes.
- Mention that controllers call IMediator and do not access DbContext directly.
