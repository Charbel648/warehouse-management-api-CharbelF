# Exercise 07 Prompt History — Shipment Tracking Module Architecture Design

## Prompt sent to AI

You are designing a new Shipment Tracking Module for an ASP.NET Core Warehouse Management API.

Current architecture:
- ASP.NET Core Controllers in Presentation
- MediatR commands and queries in Application
- Domain models and repository interfaces in Domain
- EF Core repositories in Infrastructure
- Firebase authorization policies
- RabbitMQ event publishing for notifications
- Clean Architecture rules

The warehouse system needs a Shipment Tracking Module with these features:
- create shipment
- assign products to shipment
- track shipment status
- update delivery state
- notify supplier

Design:
1. Domain models
2. Controllers
3. DTOs
4. Application commands and queries
5. Application handlers
6. Repository interfaces
7. Infrastructure repository implementation
8. Supplier notification flow
9. Folder structure

Architecture constraints:
- Do not inject DbContext into controllers.
- Do not create synchronous calls to the Notification Service.
- Use async messaging for supplier notifications.
- Keep controllers thin.
- Use IMediator in controllers.
- Keep business rules inside domain models.
- Use DTOs for requests and responses.
- Preserve the existing Warehouse API architecture.
- This is architecture/design only, not production implementation.
