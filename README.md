The project is about managing a warehouse containing products and suppliers.
The data for now is only in-memory so later we need to create a db and establish a connection.

Structure:

Contracts
Controllers
Data
Models
Services
uploads

You can find below the tree of this project :

C:.
├───.idea
├───Warehouse.Application
│   ├───Contracts
│   ├───Mapping
│   ├───Products
│   │   ├───Commands
│   │   └───Queries
│   └───Suppliers
│       ├───Commands
│       └───Queries
├───Warehouse.Domain
│   ├───Models
│   └───Repositories
├───Warehouse.Infrastructure
│   ├───Data
│   └───Repositories
└───Warehouse.Presentation
├───Contracts
├───Controllers
├───Properties
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

The product must exist.
The supplier must exist.
The product must not be archived.

So in general this is an In-memory warehouse management project where you can get, add, update and delete products and suppliers where all the endpoints have validations and requirements.
