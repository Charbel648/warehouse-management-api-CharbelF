# Exercise 04 Prompt History — AI Integration Testing

## Prompt sent to AI

You are assisting with an ASP.NET Core Warehouse Management API.

The project already uses:
- ASP.NET Core controllers
- MediatR/CQRS handlers
- Clean Architecture
- WebApplicationFactory integration tests
- seeded in-memory test store inside the integration test project

Generate integration tests using WebApplicationFactory and HttpClient for these routes:

1. POST /api/products
2. POST /api/products/{id}/image
3. DELETE /api/products/{id}

Required assertions:
- HTTP status codes
- response headers
- model property matches
- persistent server-side effects

Rules:
- Do not use a real PostgreSQL container.
- Do not depend on local Redis, MinIO, RabbitMQ, or Firebase.
- Reuse the existing CustomWebApplicationFactory.
- Keep tests isolated and deterministic.
- Do not modify production controllers just to satisfy tests.
- Use FluentAssertions.
- Use multipart form-data for image upload.
- Verify product creation persists.
- Verify image upload persists file metadata.
- Verify delete archives product and does not physically remove it.
