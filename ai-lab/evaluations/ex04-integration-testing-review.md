# Exercise 04 — AI Integration Testing Review

## Routes Covered

- POST /api/products
- POST /api/products/{id}/image
- DELETE /api/products/{id}

## Required Assertions Covered

| Requirement | Covered? | Evidence |
|---|---:|---|
| HTTP status codes | Y | Tests assert 201 Created, 200 OK |
| Response headers | Y | Tests assert Location header for product creation and JSON content type for response bodies |
| Model property matches | Y | Tests assert id, name, SKU, price, quantity, file metadata, and IsArchived |
| Persistent server-side effects | Y | Tests verify product and file metadata in TestWarehouseStore and confirm archived products remain retrievable |

## Architecture Review

The integration tests use the real ASP.NET Core HTTP pipeline through WebApplicationFactory.

External infrastructure dependencies are replaced inside the test factory only:
- PostgreSQL is replaced by the test store/repositories.
- MinIO is replaced by a test object storage service.
- RabbitMQ is replaced by a no-op event publisher.
- Firebase is replaced by test authentication.

This keeps tests stable and repeatable while preserving the controller, middleware, authorization, MediatR, and handler flow.

## AI Output Issues Found

The AI initially focused mostly on status codes and did not sufficiently verify side effects.

Manual improvements:
- Added Location header assertion for POST /api/products.
- Added response JSON content type checks.
- Verified product persistence in the server-side test store.
- Verified file metadata persistence after image upload.
- Verified delete performs archive behavior rather than physical deletion.

## Verification Command

dotnet test .\warehouse_management.sln

Expected result:

All tests pass.
