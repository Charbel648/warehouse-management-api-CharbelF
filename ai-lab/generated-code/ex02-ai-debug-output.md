# Exercise 02 AI Debugging Output — Assign Supplier Archived Product Bug

## AI Diagnosis

The risky sequence is in the assign-supplier flow:

1. The handler loads the product.
2. The handler loads the supplier.
3. The handler calls product.AssignSupplier(supplier).
4. The handler persists the updated product.

If Product.AssignSupplier does not check whether the product is archived, an archived historical product can be mapped to a live active supplier.

## Faulty Block Identified by AI

The critical block is:

product.AssignSupplier(supplier);
await _productRepository.UpdateAsync(product);

If Product.AssignSupplier does not enforce the archived-product rule before mutating SupplierId and SupplierName, the invalid assignment will be saved.

## Correct Fix Recommended by AI

The fix belongs in the Product domain aggregate, not in the controller.

Correct domain method:

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

private void EnsureNotArchived()
{
    if (IsArchived)
        throw new InvalidOperationException("Archived products cannot be updated");
}

## Human Review

The current codebase already contains this domain-level fix.

No controller-level or DbContext-level changes are needed.

The safest action is to add an isolated unit test proving:
- archived product assignment throws InvalidOperationException
- product repository UpdateAsync is never called
- supplier repository is still mockable
- no infrastructure dependency is needed

## Rejected AI Alternatives

Rejected:
- Adding DbContext checks in ProductsController.
- Adding a new ProductService that does not exist in the current architecture.
- Returning BadRequest directly inside the handler.
- Silently ignoring archived products instead of throwing a domain exception.

Accepted:
- Keep the rule in Product.AssignSupplier.
- Keep AssignSupplierToProductHandler dependent only on IProductRepository and ISupplierRepository.
- Add a focused unit test.
