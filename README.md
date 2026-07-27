The project is about managing a warehouse containing products and suppliers.

Structure:
C:.
├───.idea
├───Warehouse.Application
│   ├───Common
│   ├───Mapping
│   ├───Products
│   │   ├───Commands
│   │   │   ├───AddProductImage
│   │   │   ├───ArchiveProduct
│   │   │   ├───AssignSupplierToProduct
│   │   │   ├───CreateProduct
│   │   │   ├───UpdateProductPrice
│   │   │   └───UpdateProductQuantity
│   │   └───Queries
│   │       ├───GetProductById
│   │       ├───ListProducts
│   │       └───SearchProducts
│   ├───Suppliers
│   │   ├───Commands
│   │   │   ├───CreateSupplier
│   │   │   └───DeactivateSupplier
│   │   └───Queries
│   │       ├───GetSupplierById
│   │       └───ListSuppliers
│   └───ViewModels
├───Warehouse.Domain
│   ├───Exceptions
│   ├───Models
│   └───Repositories
├───Warehouse.Infrastructure
│   ├───Data
│   ├───Persistence
│   │   └───Migrations
│   └───Repositories
└───Warehouse.Presentation
├───Contracts
├───Controllers
├───Filters
├───Middleware
├───Properties
├───Responses
├───Services
└───wwwroot
└───uploads



You can find all the endpoints of the product below:

GET    /api/products
GET    /api/products/{id}
GET    /api/products/search
GET    /api/products/server-time
POST   /api/products
PUT   /api/products/{id}/quantity  in the lab this endpoint was supposed to be a POST methode ,but I chose to make it a PUT methode instead
PUT   /api/products/{id}/price    in the lab this endpoint was supposed to be a POST methode ,but I chose to make it a PUT methode instead
POST   /api/products/{id}/image
POST   /api/products/{id}/assign-supplier/{supplierId}
DELETE /api/products/{id}

And these are all the suppliers endpoints:

GET    /api/suppliers
GET    /api/suppliers/{id}
POST   /api/suppliers
DELETE /api/suppliers/{id}

POST    /api/stock-adjustments

GET     /api/inventory/dashboard

The endpoint POST   /api/products/{id}/assign-supplier/{supplierId} adds the ability to assign a supplier to a product
and includes the following validations:



Swagger Testing
Swagger was used to test the main API endpoints.
Tested endpoints include:

List products
Get product by id
Create product
Update product quantity
Update product price
Upload product image
Archive product
List suppliers
Create supplier
Deactivate supplier
Assign supplier to product
stock-adjustments
inventory dashboard
metadata validation



The product must exist.
The supplier must exist.
The product must not be archived.

So in general, this is a hardened Warehouse Management API where users can manage products, suppliers, and stock adjustments.
The project now supports creating, reading, updating, archiving, and assigning suppliers to products, while also including validation, consistent error responses, global exception handling, request tracking, action logging, async processing, an inventory dashboard, and validation metadata inspection using reflection.

the warehouse system was extended with a separate Notification Service using RabbitMQ for asynchronous communication between services.

The Warehouse API publishes business events when important actions happen, such as low stock detection or warehouse file uploads. The Notification Service consumes these events, stores notifications in its own database, and exposes endpoints to view and mark notifications as read.

This follows a microservice-style communication pattern where services do not directly call each other or share the same notification database.

The repository now contains two separate solutions:

warehouse-management-api-CharbelF/
├── warehouse_management.sln
├── Warehouse.Domain/
├── Warehouse.Application/
├── Warehouse.Infrastructure/
├── Warehouse.Presentation/
│
├── Warehouse.Notifications/
│   ├── Warehouse.Notifications.sln
│   ├── Warehouse.Notifications.Api/
│   ├── Warehouse.Notifications.Domain/
│   ├── Warehouse.Notifications.Application/
│   └── Warehouse.Notifications.Infrastructure/
│
└── docker-compose.rabbitmq.yml