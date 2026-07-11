using MediatR;
using Warehouse.Domain.Repositories;

namespace Warehouse.Application.Products.Commands.AssignSupplierToProduct;

public class AssignSupplierToProductHandler : IRequestHandler<AssignSupplierToProductCommand, AssignSupplierToProductResponse?>
{
    private readonly IProductRepository _productRepository;
    private readonly ISupplierRepository _supplierRepository;

    public AssignSupplierToProductHandler(
        IProductRepository productRepository,
        ISupplierRepository supplierRepository)
    {
        _productRepository = productRepository;
        _supplierRepository = supplierRepository;
    }

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
}
