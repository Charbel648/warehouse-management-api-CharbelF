# Exercise 02 Prompt History — AI Debugging Assign Supplier Bug

## Prompt sent to AI

You are reviewing an ASP.NET Core Warehouse Management API using Clean Architecture and MediatR/CQRS.

The business bug is:

Archived or historical products must never be assignable to active suppliers.

Analyze the current assign-supplier logic and output:

1. A clear plain-text explanation of the faulty logic sequence.
2. The precise line numbers or code blocks containing the logical oversight.
3. A robust high-performance C# fix conforming to domain constraints.
4. An isolated unit test verifying that mapping a supplier to an archived product safely throws an exception.
5. Verification that repository.UpdateAsync is not called when the product is archived.

Architecture rules:
- Keep controller thin.
- Do not inject DbContext into controller.
- Do not create a random ProductService if the codebase already uses MediatR/CQRS.
- Preserve domain constraints inside the Product aggregate.
- Do not use .Result or .Wait().
- Do not bypass Product.AssignSupplier().
- Use Moq and FluentAssertions for unit tests.

Current relevant code blocks:

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

Product.EnsureNotArchived:

private void EnsureNotArchived()
{
    if (IsArchived)
        throw new InvalidOperationException("Archived products cannot be updated");
}

AssignSupplierToProductHandler:

public async Task<AssignSupplierToProductResponse?> Handle(
    AssignSupplierToProductCommand request,
    CancellationToken cancellationToken)
{
    var product = await _productRepository.GetByIdAsync(request.ProductId);

    if (product == null)
        return null;

    var supplier = await _supplierRepository.GetByIdAsync(request.SupplierId);

    if (supplier == null)
        throw new InvalidOperationException("Supplier not found");

    product.AssignSupplier(supplier);

    await _productRepository.UpdateAsync(product);

    return new AssignSupplierToProductResponse
    {
        ProductId = product.Id,
        ProductName = product.Name,
        SupplierId = supplier.Id,
        SupplierName = supplier.Name,
        LastUpdatedAt = product.LastUpdatedAt
    };
}
