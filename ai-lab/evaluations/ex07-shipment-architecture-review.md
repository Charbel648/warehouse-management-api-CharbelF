# Exercise 07 — Shipment Tracking Module Architecture Review

## Requirement

Design a greenfield Shipment Tracking Module for the warehouse system.

Required features:

- create shipment
- assign products
- track status
- update delivery state
- notify supplier

Required design outputs:

- domain models
- controllers
- services or application layer use cases
- DTOs
- folder structure

## Architecture Fit

The AI initially described a service-based design. Human review adjusted it to match the current codebase, which uses MediatR/CQRS.

Final architecture:

```text
ShipmentsController
→ IMediator
→ Shipment Commands / Queries
→ Shipment Handlers
→ IShipmentRepository
→ ShipmentRepository
→ WarehouseDbContext
```

## Domain Model Review

| Model | Purpose | Accepted? | Notes |
|---|---|---:|---|
| Shipment | Main aggregate for shipment lifecycle | Y | Owns status and delivery state rules |
| ShipmentItem | Child entity for products assigned to shipment | Y | Prevents duplicated product assignment inside shipment |
| ShipmentStatus | Tracks business lifecycle | Y | Draft, ReadyToShip, Shipped, Delivered, Cancelled |
| DeliveryState | Tracks physical delivery progress | Y | Pending, InTransit, Delayed, Delivered, Failed |

## Controller Review

The proposed controller follows existing rules:

- thin controller
- uses IMediator
- no DbContext injection
- no repository injection
- no RabbitMQ client injection
- authorization policies applied by endpoint type

Accepted.

## DTO Review

DTOs are required because controllers should not expose raw domain entities.

Accepted request DTOs:

- CreateShipmentRequest
- AddShipmentProductRequest
- UpdateShipmentStatusRequest
- UpdateDeliveryStateRequest

Accepted response DTOs:

- ShipmentViewModel
- ShipmentItemViewModel

## Notification Review

The AI initially suggested direct notification service calls.

Rejected.

Correct design:

```text
Shipment event
→ RabbitMQ
→ Notification Service
→ supplier notification record
```

This preserves service boundaries and avoids synchronous distributed coupling.

## Performance Review

The design avoids N+1 query risks by requiring shipment details to load items using EF Core Include when needed.

Read-only list queries should use AsNoTracking.

Mutation queries should remain tracked.

## Security Review

Reader endpoints should require warehouse reader policy.

Mutation endpoints should require warehouse admin policy.

No hard-coded emails or supplier IDs should be used.

No notification secrets or RabbitMQ credentials should be hard-coded inside controllers.

## Final Decision

The design is accepted because it satisfies the shipment feature requirements while preserving the current backend architecture, authorization model, repository boundaries, and async notification pattern.
