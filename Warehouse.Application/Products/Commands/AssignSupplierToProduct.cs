using Warehouse.Application.Contracts;
using Warehouse.Application.Mapping;
using Warehouse.Domain.Repositories;

namespace Warehouse.Application.Products.Commands;

public class AssignSupplierToProduct
{
    private readonly IProductRepository _productRepository;
    private readonly ISupplierRepository _supplierRepository;

    public AssignSupplierToProduct(
        IProductRepository productRepository,
        ISupplierRepository supplierRepository)
    {
        _productRepository = productRepository;
        _supplierRepository = supplierRepository;
    }

    public async Task<ProductDto?> ExecuteAsync(string productId, string supplierId)
    {
        var product = await _productRepository.GetByIdAsync(productId);

        if (product == null)
        {
            return null;
        }

        var supplier = await _supplierRepository.GetByIdAsync(supplierId);

        if (supplier == null)
        {
            throw new InvalidOperationException("Supplier not found");
        }

        product.AssignSupplier(supplier);

        await _productRepository.UpdateAsync(product);

        return WarehouseMapper.ToDto(product);
    }
}