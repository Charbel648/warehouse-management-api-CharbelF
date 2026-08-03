using Warehouse.Domain.Models;

namespace Warehouse.Api.UnitTests.Builders;

public class ProductBuilder
{
    private string _name = "Laptop";
    private string _sku = "SKU-001";
    private string _description = "Test product";
    private decimal _price = 100;
    private int _quantityInStock = 10;
    private string _supplierName = "Default Supplier";
    private DateTime _expiryDate = DateTime.UtcNow.AddYears(1);

    public ProductBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public ProductBuilder WithSku(string sku)
    {
        _sku = sku;
        return this;
    }

    public ProductBuilder WithSupplierName(string supplierName)
    {
        _supplierName = supplierName;
        return this;
    }

    public ProductBuilder WithQuantity(int quantity)
    {
        _quantityInStock = quantity;
        return this;
    }

    public ProductBuilder WithPrice(decimal price)
    {
        _price = price;
        return this;
    }

    public Product Build()
    {
        return new Product(
            _name,
            _sku,
            _description,
            _price,
            _quantityInStock,
            _supplierName,
            _expiryDate
        );
    }
}
