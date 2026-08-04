# Exercise 07 — AI-Generated Shipment Tracking Module Design

## Module Goal

The Shipment Tracking Module adds shipment lifecycle management to the Warehouse Management API.

Required capabilities:

- create shipment
- assign products to shipment
- track shipment status
- update delivery state
- notify supplier

This exercise is architecture/design only. No production implementation is added.

---

# 1. Architecture Decision

The existing project uses Clean Architecture and MediatR/CQRS.

Correct shipment architecture:

```text
ShipmentsController
→ IMediator
→ Application Commands / Queries
→ Application Handlers
→ Domain Models
→ IShipmentRepository
→ ShipmentRepository
→ WarehouseDbContext
```

The module must not inject `WarehouseDbContext` into controllers.

The module must not call the Notification Service synchronously.

Supplier notification should happen through async event publishing.

---

# 2. Domain Models

## Shipment

Suggested file:

```text
Warehouse.Domain/Models/Shipment.cs
```

Suggested responsibility:

The `Shipment` aggregate owns shipment lifecycle rules.

Suggested properties:

```csharp
public class Shipment
{
    public string ShipmentId { get; private set; } = Guid.NewGuid().ToString();

    public string Id => ShipmentId;

    public string ReferenceNumber { get; private set; } = string.Empty;

    public string SupplierId { get; private set; } = string.Empty;

    public ShipmentStatus Status { get; private set; }

    public DeliveryState DeliveryState { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime LastUpdatedAt { get; private set; }

    public DateTime? ShippedAt { get; private set; }

    public DateTime? DeliveredAt { get; private set; }

    public List<ShipmentItem> Items { get; private set; } = new();
}
```

Suggested domain methods:

```csharp
public void AddProduct(string productId, int quantity)

public void MarkReadyToShip()

public void MarkShipped()

public void MarkDelivered()

public void MarkCancelled()

public void UpdateDeliveryState(DeliveryState deliveryState)
```

Business rules:

- ReferenceNumber is required.
- SupplierId is required.
- A shipment cannot be shipped without products.
- A delivered shipment cannot be modified.
- A cancelled shipment cannot be shipped or delivered.
- Product quantities must be greater than zero.
- The same product should not be duplicated inside the same shipment.

---

## ShipmentItem

Suggested file:

```text
Warehouse.Domain/Models/ShipmentItem.cs
```

Suggested responsibility:

Represents a product assigned to a shipment.

Suggested properties:

```csharp
public class ShipmentItem
{
    public string ShipmentItemId { get; private set; } = Guid.NewGuid().ToString();

    public string Id => ShipmentItemId;

    public string ShipmentId { get; private set; } = string.Empty;

    public string ProductId { get; private set; } = string.Empty;

    public int Quantity { get; private set; }
}
```

Business rules:

- ProductId is required.
- Quantity must be greater than zero.
- ShipmentItem belongs to one Shipment.

---

## ShipmentStatus

Suggested file:

```text
Warehouse.Domain/Models/ShipmentStatus.cs
```

```csharp
public enum ShipmentStatus
{
    Draft = 1,
    ReadyToShip = 2,
    Shipped = 3,
    Delivered = 4,
    Cancelled = 5
}
```

---

## DeliveryState

Suggested file:

```text
Warehouse.Domain/Models/DeliveryState.cs
```

```csharp
public enum DeliveryState
{
    Pending = 1,
    InTransit = 2,
    Delayed = 3,
    Delivered = 4,
    Failed = 5
}
```

---

# 3. Repository Interface

Suggested file:

```text
Warehouse.Domain/Repositories/IShipmentRepository.cs
```

Suggested interface:

```csharp
public interface IShipmentRepository
{
    Task<List<Shipment>> GetAllAsync(CancellationToken cancellationToken);

    Task<Shipment?> GetByIdAsync(
        string shipmentId,
        CancellationToken cancellationToken);

    Task AddAsync(
        Shipment shipment,
        CancellationToken cancellationToken);

    Task UpdateAsync(
        Shipment shipment,
        CancellationToken cancellationToken);
}
```

Repository rules:

- Keep EF Core inside Infrastructure only.
- Do not expose IQueryable outside Infrastructure.
- Use `AsNoTracking()` for read-only list queries.
- Keep tracked queries for updates.
- Use `Include(shipment => shipment.Items)` when shipment details require items.

---

# 4. Application Layer

Suggested folder:

```text
Warehouse.Application/Shipments/
```

Suggested command folders:

```text
Warehouse.Application/Shipments/Commands/CreateShipment/
Warehouse.Application/Shipments/Commands/AddProductToShipment/
Warehouse.Application/Shipments/Commands/UpdateShipmentStatus/
Warehouse.Application/Shipments/Commands/UpdateDeliveryState/
```

Suggested query folders:

```text
Warehouse.Application/Shipments/Queries/ListShipments/
Warehouse.Application/Shipments/Queries/GetShipmentById/
```

---

## CreateShipmentCommand

```csharp
public class CreateShipmentCommand : IRequest<ShipmentViewModel>
{
    public string SupplierId { get; set; } = string.Empty;

    public string ReferenceNumber { get; set; } = string.Empty;
}
```

Handler responsibilities:

- Validate supplier exists.
- Create Shipment domain object.
- Save through IShipmentRepository.
- Publish `shipment.created` event.
- Return ShipmentViewModel.

---

## AddProductToShipmentCommand

```csharp
public class AddProductToShipmentCommand : IRequest<ShipmentViewModel>
{
    public string ShipmentId { get; set; } = string.Empty;

    public string ProductId { get; set; } = string.Empty;

    public int Quantity { get; set; }
}
```

Handler responsibilities:

- Load shipment.
- Load product.
- Validate product exists and is not archived.
- Call shipment.AddProduct(productId, quantity).
- Save shipment.
- Publish `shipment.product-assigned` event.
- Return ShipmentViewModel.

---

## UpdateShipmentStatusCommand

```csharp
public class UpdateShipmentStatusCommand : IRequest<ShipmentViewModel>
{
    public string ShipmentId { get; set; } = string.Empty;

    public ShipmentStatus Status { get; set; }
}
```

Handler responsibilities:

- Load shipment.
- Apply status change through domain method.
- Save shipment.
- Publish `shipment.status-updated` event.
- Return ShipmentViewModel.

---

## UpdateDeliveryStateCommand

```csharp
public class UpdateDeliveryStateCommand : IRequest<ShipmentViewModel>
{
    public string ShipmentId { get; set; } = string.Empty;

    public DeliveryState DeliveryState { get; set; }
}
```

Handler responsibilities:

- Load shipment.
- Apply delivery state through domain method.
- Save shipment.
- Publish `shipment.delivery-state-updated` event.
- Return ShipmentViewModel.

---

# 5. ViewModels

Suggested folder:

```text
Warehouse.Application/Shipments/ViewModels/
```

## ShipmentViewModel

```csharp
public class ShipmentViewModel
{
    public string Id { get; set; } = string.Empty;

    public string ReferenceNumber { get; set; } = string.Empty;

    public string SupplierId { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string DeliveryState { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime LastUpdatedAt { get; set; }

    public List<ShipmentItemViewModel> Items { get; set; } = new();
}
```

## ShipmentItemViewModel

```csharp
public class ShipmentItemViewModel
{
    public string ProductId { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public int Quantity { get; set; }
}
```

---

# 6. Presentation Layer Contracts

Suggested folder:

```text
Warehouse.Presentation/Contracts/Shipments/
```

## CreateShipmentRequest

```csharp
public class CreateShipmentRequest
{
    [Required]
    public string SupplierId { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string ReferenceNumber { get; set; } = string.Empty;
}
```

## AddShipmentProductRequest

```csharp
public class AddShipmentProductRequest
{
    [Required]
    public string ProductId { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}
```

## UpdateShipmentStatusRequest

```csharp
public class UpdateShipmentStatusRequest
{
    [EnumDataType(typeof(ShipmentStatus))]
    public ShipmentStatus Status { get; set; }
}
```

## UpdateDeliveryStateRequest

```csharp
public class UpdateDeliveryStateRequest
{
    [EnumDataType(typeof(DeliveryState))]
    public DeliveryState DeliveryState { get; set; }
}
```

---

# 7. API Controller Design

Suggested file:

```text
Warehouse.Presentation/Controllers/ShipmentsController.cs
```

Suggested routes:

```http
GET /api/shipments
GET /api/shipments/{id}
POST /api/shipments
POST /api/shipments/{id}/products
PUT /api/shipments/{id}/status
PUT /api/shipments/{id}/delivery-state
```

Suggested authorization:

Reader policy:

```text
GET /api/shipments
GET /api/shipments/{id}
```

Admin policy:

```text
POST /api/shipments
POST /api/shipments/{id}/products
PUT /api/shipments/{id}/status
PUT /api/shipments/{id}/delivery-state
```

Controller responsibilities:

- Accept HTTP request.
- Use model binding and validation.
- Send commands/queries through IMediator.
- Return HTTP responses.
- Do not access DbContext directly.
- Do not publish RabbitMQ messages directly.

---

# 8. Supplier Notification Flow

The Shipment module should notify suppliers asynchronously.

Correct flow:

```text
Shipment action occurs
→ Shipment handler publishes warehouse notification event
→ RabbitMQ
→ Notification Service consumes event
→ Notification Service stores supplier notification
```

Suggested event types:

```text
shipment.created
shipment.product-assigned
shipment.status-updated
shipment.delivery-state-updated
```

Suggested event payload:

```json
{
  "eventType": "shipment.status-updated",
  "entityId": "shipment-guid",
  "entityType": "shipment",
  "supplierId": "supplier-guid",
  "title": "Shipment status updated",
  "message": "Shipment SHIP-001 changed to Shipped.",
  "severity": "information",
  "occurredAt": "2026-08-04T10:00:00Z"
}
```

Why async messaging is preferred:

- Warehouse API remains independent.
- Notification Service remains independent.
- A temporary notification failure does not break shipment update.
- RabbitMQ keeps the integration consistent with the existing project.

---

# 9. Suggested Folder Structure

```text
Warehouse.Domain/
  Models/
    Shipment.cs
    ShipmentItem.cs
    ShipmentStatus.cs
    DeliveryState.cs
  Repositories/
    IShipmentRepository.cs

Warehouse.Application/
  Shipments/
    Commands/
      CreateShipment/
        CreateShipmentCommand.cs
        CreateShipmentHandler.cs
      AddProductToShipment/
        AddProductToShipmentCommand.cs
        AddProductToShipmentHandler.cs
      UpdateShipmentStatus/
        UpdateShipmentStatusCommand.cs
        UpdateShipmentStatusHandler.cs
      UpdateDeliveryState/
        UpdateDeliveryStateCommand.cs
        UpdateDeliveryStateHandler.cs
    Queries/
      ListShipments/
        ListShipmentsQuery.cs
        ListShipmentsHandler.cs
      GetShipmentById/
        GetShipmentByIdQuery.cs
        GetShipmentByIdHandler.cs
    ViewModels/
      ShipmentViewModel.cs
      ShipmentItemViewModel.cs

Warehouse.Infrastructure/
  Repositories/
    ShipmentRepository.cs
  Persistence/
    Configurations/
      ShipmentConfiguration.cs
      ShipmentItemConfiguration.cs

Warehouse.Presentation/
  Contracts/
    Shipments/
      CreateShipmentRequest.cs
      AddShipmentProductRequest.cs
      UpdateShipmentStatusRequest.cs
      UpdateDeliveryStateRequest.cs
  Controllers/
    ShipmentsController.cs

tests/
  Warehouse.Api.UnitTests/
    Shipments/
      CreateShipmentHandlerTests.cs
      AddProductToShipmentHandlerTests.cs
      UpdateShipmentStatusHandlerTests.cs
      UpdateDeliveryStateHandlerTests.cs
  Warehouse.Api.IntegrationTests/
    Shipments/
      ShipmentEndpointTests.cs
```

---

# 10. Suggested Tests

## Unit Tests

CreateShipmentHandlerTests:

- valid shipment should be created
- missing supplier should fail
- empty reference number should fail

AddProductToShipmentHandlerTests:

- valid product should be assigned
- archived product should fail
- zero quantity should fail
- duplicated product should fail

UpdateShipmentStatusHandlerTests:

- draft shipment can become ready to ship
- empty shipment cannot be shipped
- delivered shipment cannot be cancelled
- cancelled shipment cannot be shipped

UpdateDeliveryStateHandlerTests:

- shipment can move to InTransit
- shipment can move to Delayed
- shipment can move to Delivered
- delivered shipment cannot return to Pending

## Integration Tests

ShipmentEndpointTests:

- POST /api/shipments returns 201 Created and persists shipment
- POST /api/shipments/{id}/products returns 200 OK and persists item
- PUT /api/shipments/{id}/status updates status
- PUT /api/shipments/{id}/delivery-state updates delivery state
- GET /api/shipments/{id} returns shipment with items
- unauthorized request returns 401 or 403

---

# 11. Security and Validation Notes

- Use authorization policies on all shipment routes.
- Validate IDs as GUID route values where possible.
- Do not expose internal database IDs beyond API contract.
- Do not hard-code supplier IDs.
- Do not hard-code notification recipients.
- Do not store RabbitMQ credentials in source code.
- Do not expose internal exception details to clients.

---

# 12. AI Output Review

Accepted AI suggestions:

- Shipment aggregate.
- ShipmentItem child entity.
- Status and delivery state enums.
- Controller using IMediator.
- Application commands and queries.
- Repository abstraction.
- Async supplier notification through RabbitMQ.
- DTO-based request and response models.

Rejected AI suggestions:

- Direct DbContext usage inside ShipmentsController.
- Direct synchronous call to Notification Service.
- Creating notification database records inside Warehouse API.
- Putting business rules inside the controller.
- Returning EF Core domain entities directly from controller.
- Adding a ProductService just for this module.

---

# 13. Final Design Decision

The Shipment Tracking Module design is accepted.

Reason:

It satisfies the requested module capabilities while preserving the existing backend architecture, authorization strategy, repository boundaries, MediatR usage, and asynchronous notification pattern.
