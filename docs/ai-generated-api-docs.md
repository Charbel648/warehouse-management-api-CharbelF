# AI-Generated API Documentation — Warehouse Management API

## Overview

The Warehouse Management API is an ASP.NET Core backend for managing warehouse products, suppliers, stock, files, and inventory visibility.

The project follows Clean Architecture:

```text
Presentation Layer
ASP.NET Core Controllers

Application Layer
MediatR Commands / Queries / Handlers

Domain Layer
Entities, business rules, repository interfaces

Infrastructure Layer
EF Core repositories, external storage, messaging
```

Controllers remain thin and call `IMediator`. They do not access `DbContext`, repositories, MinIO, RabbitMQ, or Firebase directly.

---

## Authorization

Most endpoints require Firebase JWT authentication.

Example:

```http
Authorization: Bearer <firebase_id_token>
```

Reader endpoints require the warehouse reader policy.

Admin endpoints require the warehouse admin policy.

---

# Product Endpoints

## Get all products

```http
GET /api/products
```

### Query Parameters

| Name | Type | Required | Description |
|---|---|---:|---|
| onlyAvailable | boolean | No | Filters available products if true |

### Authorization

Warehouse reader policy.

### Example Request

```http
GET /api/products?onlyAvailable=false
Authorization: Bearer <firebase_id_token>
```

### Example Response

```json
[
  {
    "id": "product-guid",
    "name": "Laptop",
    "sku": "SKU-001",
    "description": "Gaming laptop",
    "price": 1200,
    "quantityInStock": 5,
    "supplierName": "Tech Supplier",
    "expiryDate": "2027-08-04T00:00:00Z",
    "isArchived": false,
    "createdAt": "2026-08-04T10:00:00Z",
    "lastUpdatedAt": "2026-08-04T10:00:00Z"
  }
]
```

---

## Get product by id

```http
GET /api/products/{id}
```

### Authorization

Warehouse reader policy.

### Example Request

```http
GET /api/products/11111111-1111-1111-1111-111111111111
Authorization: Bearer <firebase_id_token>
```

### Example Response

```json
{
  "id": "11111111-1111-1111-1111-111111111111",
  "name": "Laptop",
  "sku": "SKU-001",
  "description": "Gaming laptop",
  "price": 1200,
  "quantityInStock": 5,
  "supplierName": "Tech Supplier",
  "expiryDate": "2027-08-04T00:00:00Z",
  "isArchived": false,
  "createdAt": "2026-08-04T10:00:00Z",
  "lastUpdatedAt": "2026-08-04T10:00:00Z"
}
```

---

## Search products

```http
GET /api/products/search
```

### Query Parameters

| Name | Type | Required | Description |
|---|---|---:|---|
| name | string | No | Product name filter |
| supplier | string | No | Supplier name filter |

At least one filter should be provided.

### Authorization

Warehouse reader policy.

### Example Request

```http
GET /api/products/search?name=Laptop&supplier=Tech
Authorization: Bearer <firebase_id_token>
```

### Example Response

```json
[
  {
    "id": "product-guid",
    "name": "Laptop",
    "sku": "SKU-001",
    "supplierName": "Tech Supplier"
  }
]
```

---

## Get expiring soon products

```http
GET /api/products/expiring-soon
```

### Summary

Returns non-archived products whose expiry date is between today and the next 30 calendar days.

Already expired products are excluded.

Archived products are excluded.

### Authorization

Warehouse reader policy.

### Example Request

```http
GET /api/products/expiring-soon
Authorization: Bearer <firebase_id_token>
```

### Example Response

```json
[
  {
    "id": "product-guid",
    "name": "Milk",
    "sku": "MILK-001",
    "quantityInStock": 20,
    "supplierName": "Food Supplier",
    "expiryDate": "2026-08-14T00:00:00Z",
    "daysUntilExpiry": 10
  }
]
```

### Architecture Flow

```text
ProductsController
→ IMediator
→ GetExpiringSoonProductsQuery
→ GetExpiringSoonProductsHandler
→ IProductRepository.GetExpiredOrExpiringProductsAsync
→ ProductRepository
```

---

## Create product

```http
POST /api/products
```

### Authorization

Warehouse admin policy.

### Request Body

```json
{
  "name": "Laptop",
  "sku": "SKU-001",
  "description": "Gaming laptop",
  "price": 1200,
  "quantityInStock": 5,
  "supplierName": "Tech Supplier",
  "expiryDate": "2027-08-04T00:00:00Z"
}
```

### Success Response

Returns `201 Created`.

```json
{
  "id": "product-guid",
  "name": "Laptop",
  "sku": "SKU-001",
  "description": "Gaming laptop",
  "price": 1200,
  "quantityInStock": 5,
  "supplierName": "Tech Supplier",
  "expiryDate": "2027-08-04T00:00:00Z",
  "isArchived": false
}
```

### Conflict Response

Duplicate SKU returns `409 Conflict`.

```json
{
  "errorCode": "conflict",
  "message": "A product with the same SKU already exists",
  "traceId": "trace-id"
}
```

---

## Update product quantity

```http
PUT /api/products/{id}/quantity
```

### Authorization

Warehouse admin policy.

### Request Body

```json
{
  "quantityInStock": 3
}
```

### Example Response

```json
{
  "id": "product-guid",
  "name": "Laptop",
  "quantityInStock": 3,
  "lastUpdatedAt": "2026-08-04T10:00:00Z"
}
```

### Notes

Quantity cannot be negative.

Low stock may publish a warehouse notification event.

---

## Update product price

```http
PUT /api/products/{id}/price
```

### Authorization

Warehouse admin policy.

### Request Body

```json
{
  "price": 250.75
}
```

### Example Response

```json
{
  "id": "product-guid",
  "name": "Laptop",
  "price": 250.75,
  "lastUpdatedAt": "2026-08-04T10:00:00Z"
}
```

### Notes

Price must be greater than zero.

---

## Upload product image

```http
POST /api/products/{id}/image
```

### Authorization

Warehouse admin policy.

### Content Type

```http
multipart/form-data
```

### Form Field

| Field | Type | Required |
|---|---|---:|
| Image | file | Yes |

### Supported Files

- JPG
- JPEG
- PNG

Maximum size:

```text
2 MB
```

### Example curl

```bash
curl -X POST "http://localhost:5028/api/products/<product_id>/image" \
  -H "Authorization: Bearer <firebase_id_token>" \
  -F "Image=@product-image.jpg;type=image/jpeg"
```

### Example Response

```json
{
  "fileId": "file-guid",
  "relatedEntityId": "product-guid",
  "relatedEntityType": "product",
  "fileCategory": "product-image",
  "originalFileName": "product-image.jpg",
  "contentType": "image/jpeg",
  "sizeInBytes": 1024,
  "uploadedAt": "2026-08-04T10:00:00Z"
}
```

---

## Archive product

```http
DELETE /api/products/{id}
```

### Authorization

Warehouse admin policy.

### Summary

Archives the product instead of physically deleting it.

### Example Response

```json
{
  "id": "product-guid",
  "name": "Laptop",
  "isArchived": true,
  "lastUpdatedAt": "2026-08-04T10:00:00Z"
}
```

---

## Assign supplier to product

```http
POST /api/products/{id}/assign-supplier/{supplierId}
```

### Authorization

Warehouse admin policy.

### Summary

Assigns an active supplier to a non-archived product.

### Example Response

```json
{
  "productId": "product-guid",
  "productName": "Laptop",
  "supplierId": "supplier-guid",
  "supplierName": "Tech Supplier",
  "lastUpdatedAt": "2026-08-04T10:00:00Z"
}
```

### Business Rules

- Archived products cannot be updated.
- Inactive suppliers cannot be assigned to products.
- Missing supplier returns an error.

---

## Get server time

```http
GET /api/products/server-time
```

### Headers

```http
Accept-Language: en-US
```

Supported languages:

```text
en-US
fr-FR
ar-LB
```

### Example Response

```json
{
  "language": "en-US",
  "serverTime": "Tuesday, August 4, 2026 1:00:00 PM"
}
```

---

# Supplier Endpoints

## Get all suppliers

```http
GET /api/suppliers
```

### Authorization

Warehouse reader policy.

### Example Response

```json
[
  {
    "id": "supplier-guid",
    "name": "Tech Supplier",
    "country": "Lebanon",
    "contactEmail": "supplier@test.com",
    "phoneNumber": "+96100000000",
    "isActive": true
  }
]
```

---

## Get supplier by id

```http
GET /api/suppliers/{id}
```

### Example Response

```json
{
  "id": "supplier-guid",
  "name": "Tech Supplier",
  "country": "Lebanon",
  "contactEmail": "supplier@test.com",
  "phoneNumber": "+96100000000",
  "isActive": true
}
```

---

## Create supplier

```http
POST /api/suppliers
```

### Authorization

Warehouse admin policy.

### Request Body

```json
{
  "name": "Tech Supplier",
  "country": "Lebanon",
  "contactEmail": "supplier@test.com",
  "phoneNumber": "+96100000000"
}
```

### Success Response

Returns `201 Created`.

```json
{
  "id": "supplier-guid",
  "name": "Tech Supplier",
  "country": "Lebanon",
  "contactEmail": "supplier@test.com",
  "phoneNumber": "+96100000000",
  "isActive": true
}
```

### Conflict Response

Duplicate supplier email returns `409 Conflict`.

```json
{
  "errorCode": "conflict",
  "message": "A supplier with the same email already exists",
  "traceId": "trace-id"
}
```

---

## Deactivate supplier

```http
DELETE /api/suppliers/{id}
```

### Summary

Marks the supplier inactive instead of deleting it.

### Example Response

```json
{
  "id": "supplier-guid",
  "name": "Tech Supplier",
  "isActive": false
}
```

---

## Upload supplier document

```http
POST /api/suppliers/{id}/documents
```

### Authorization

Warehouse admin policy.

### Content Type

```http
multipart/form-data
```

### Form Field

| Field | Type | Required |
|---|---|---:|
| Document | file | Yes |

### Supported Files

- PDF
- JPG
- JPEG
- PNG

Maximum size:

```text
5 MB
```

### Example curl

```bash
curl -X POST "http://localhost:5028/api/suppliers/<supplier_id>/documents" \
  -H "Authorization: Bearer <firebase_id_token>" \
  -F "Document=@supplier-document.pdf;type=application/pdf"
```

### Example Response

```json
{
  "fileId": "file-guid",
  "relatedEntityId": "supplier-guid",
  "relatedEntityType": "supplier",
  "fileCategory": "supplier-document",
  "originalFileName": "supplier-document.pdf",
  "contentType": "application/pdf",
  "sizeInBytes": 1024,
  "uploadedAt": "2026-08-04T10:00:00Z"
}
```

---

# Stock Adjustment Endpoint

## Create stock adjustment

```http
POST /api/stock-adjustments
```

### Summary

Applies a stock quantity change to a product.

### Example Request

```json
{
  "productId": "product-guid",
  "quantityChange": -2,
  "reason": "Damaged items removed from stock"
}
```

### Example Response

```json
{
  "productId": "product-guid",
  "quantityInStock": 8,
  "reason": "Damaged items removed from stock"
}
```

---

# Inventory Endpoint

## Get inventory dashboard

```http
GET /api/inventory/dashboard
```

### Summary

Returns dashboard-level inventory metrics.

### Example Response

```json
{
  "totalProducts": 25,
  "availableProducts": 20,
  "archivedProducts": 5,
  "lowStockProducts": 3
}
```

---

# File Endpoint

## Download file

```http
GET /api/files/{fileId}/download
```

### Summary

Downloads a previously uploaded warehouse file.

### Example Request

```http
GET /api/files/file-guid/download
Authorization: Bearer <firebase_id_token>
```

---

# Error Response Format

The API uses a consistent error response shape.

```json
{
  "errorCode": "not_found",
  "message": "Resource was not found",
  "traceId": "trace-id"
}
```

Common statuses:

| Status | Meaning |
|---:|---|
| 400 | Validation or business rule error |
| 401 | Missing or invalid authentication |
| 403 | Authenticated user does not have required policy |
| 404 | Resource not found |
| 409 | Conflict such as duplicate SKU or duplicate supplier email |
| 500 | Unexpected server error |

---

# Architecture Notes

## Presentation Layer

Controllers receive HTTP requests, validate route/body shape through ASP.NET Core model binding, and send commands/queries through `IMediator`.

Controllers do not directly access:

- WarehouseDbContext
- repositories
- MinIO client
- RabbitMQ client
- Firebase internals

## Application Layer

The Application layer contains command/query handlers.

Examples:

```text
CreateProductCommand → CreateProductHandler
GetExpiringSoonProductsQuery → GetExpiringSoonProductsHandler
UploadProductImageCommand → UploadProductImageCommandHandler
```

Handlers contain application use-case logic and depend on domain repository interfaces.

## Domain Layer

The Domain layer owns business rules.

Examples:

- Product price must be greater than zero.
- Product quantity cannot be negative.
- Archived products cannot be updated.
- Inactive suppliers cannot be assigned to products.

## Infrastructure Layer

The Infrastructure layer contains:

- EF Core repository implementations
- object storage implementation
- messaging/event publishing implementation

Infrastructure dependencies are hidden behind interfaces.

---

# Testing Notes

Unit tests use mocked repository boundaries.

Integration tests use `WebApplicationFactory` to execute real HTTP flows while replacing external infrastructure with test doubles for repeatability.

---

# Security Notes

- Never commit Firebase service-account files.
- Never commit API keys or tokens.
- Use generated object keys for uploaded files.
- Validate file type and size before storage.
- Do not expose physical storage paths in API responses.
- Use authorization policies for protected endpoints.
