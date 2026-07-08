namespace Warehouse.Domain.Models;

public class Product
{
    public string Id { get; private set; } = Guid.NewGuid().ToString();

    public string Name { get; private set; }

    public string SKU { get; private set; }

    public string Description { get; private set; }

    public decimal Price { get; private set; }

    public int QuantityInStock { get; private set; }

    public string SupplierId { get; private set; } = string.Empty;

    public string SupplierName { get; private set; }

    public DateTime ExpiryDate { get; private set; }

    public bool IsArchived { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime LastUpdatedAt { get; private set; }

    public List<ProductImage> Images { get; private set; } = new();

    public Product(
        string name,
        string sku,
        string description,
        decimal price,
        int quantityInStock,
        string supplierName,
        DateTime expiryDate)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name is required");

        if (string.IsNullOrWhiteSpace(sku))
            throw new ArgumentException("SKU is required");

        if (price <= 0)
            throw new ArgumentException("Price must be greater than zero");

        if (quantityInStock < 0)
            throw new ArgumentException("Quantity cannot be negative");

        Name = name;
        SKU = sku;
        Description = description;
        Price = price;
        QuantityInStock = quantityInStock;
        SupplierName = supplierName;
        ExpiryDate = expiryDate;
        CreatedAt = DateTime.UtcNow;
        LastUpdatedAt = DateTime.UtcNow;
    }

    public void UpdateQuantity(int quantity)
    {
        EnsureNotArchived();

        if (quantity < 0)
            throw new ArgumentException("Quantity cannot be negative");

        QuantityInStock = quantity;
        LastUpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePrice(decimal price)
    {
        EnsureNotArchived();

        if (price <= 0)
            throw new ArgumentException("Price must be greater than zero");

        Price = price;
        LastUpdatedAt = DateTime.UtcNow;
    }

    public void Archive()
    {
        IsArchived = true;
        LastUpdatedAt = DateTime.UtcNow;
    }

    public void AssignSupplier(Supplier supplier)
    {
        EnsureNotArchived();

        if (!supplier.IsActive)
            throw new InvalidOperationException("Inactive suppliers cannot be assigned to products");

        SupplierId = supplier.Id;
        SupplierName = supplier.Name;
        LastUpdatedAt = DateTime.UtcNow;
    }

    public void AddImage(ProductImage image)
    {
        EnsureNotArchived();

        Images.Add(image);
        LastUpdatedAt = DateTime.UtcNow;
    }

    private void EnsureNotArchived()
    {
        if (IsArchived)
            throw new InvalidOperationException("Archived products cannot be updated");
    }
}
