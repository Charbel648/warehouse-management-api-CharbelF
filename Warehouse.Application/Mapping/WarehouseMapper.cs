using Warehouse.Application.Contracts;
using Warehouse.Domain.Models;

namespace Warehouse.Application.Mapping;

public static class WarehouseMapper
{
    public static ProductDto ToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            SKU = product.SKU,
            Description = product.Description,
            Price = product.Price,
            QuantityInStock = product.QuantityInStock,
            SupplierId = product.SupplierId,
            SupplierName = product.SupplierName,
            ExpiryDate = product.ExpiryDate,
            IsArchived = product.IsArchived,
            CreatedAt = product.CreatedAt,
            LastUpdatedAt = product.LastUpdatedAt
        };
    }

    public static SupplierDto ToDto(Supplier supplier)
    {
        return new SupplierDto
        {
            Id = supplier.Id,
            Name = supplier.Name,
            Country = supplier.Country,
            ContactEmail = supplier.ContactEmail,
            PhoneNumber = supplier.PhoneNumber,
            IsActive = supplier.IsActive
        };
    }
}