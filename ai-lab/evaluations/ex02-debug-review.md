# Exercise 02 — AI Debugging Review

## Bug Scenario

Archived or historical products should not be assignable to live active suppliers.

## Current Architecture

The project does not use a ProductService class for this flow. It uses:

ProductsController
→ IMediator
→ AssignSupplierToProductHandler
→ Product.AssignSupplier
→ IProductRepository.UpdateAsync

## Faulty Logic Sequence Explained

If the Product domain method does not check archived state before assigning a supplier, the following sequence becomes unsafe:

1. Handler loads product.
2. Handler loads supplier.
3. Handler calls product.AssignSupplier(supplier).
4. Product updates SupplierId and SupplierName.
5. Handler saves the product.

The oversight would be allowing step 4 to happen even when Product.IsArchived is true.

## Exact Code Block Reviewed

AssignSupplierToProductHandler:

product.AssignSupplier(supplier);
await _productRepository.UpdateAsync(product);

Product.AssignSupplier:

public void AssignSupplier(Supplier supplier)
{
    EnsureNotArchived();

    if (!supplier.IsActive)
        throw new InvalidOperationException("Inactive suppliers cannot be assigned to products");

    SupplierId = supplier.SupplierId;
    SupplierName = supplier.Name;
    Supplier = supplier;
    LastUpdatedAt = DateTime.UtcNow;
}

## Human Verification

The current domain method already calls EnsureNotArchived() before mutating supplier fields.

This is the correct location for the rule because the product aggregate owns the invariant:
"Archived products cannot be updated."

## Test Added

tests/Warehouse.Api.UnitTests/Products/AssignSupplierArchivedProductTests.cs

The test verifies:
- archived product assignment throws InvalidOperationException
- SupplierId remains unchanged
- SupplierName remains unchanged
- repository UpdateAsync is never called

## Regression Check

Command:

dotnet test .\warehouse_management.sln

Expected result:

All tests pass.
