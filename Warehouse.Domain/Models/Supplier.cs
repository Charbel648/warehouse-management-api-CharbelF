namespace Warehouse.Domain.Models;

public class Supplier
{
    public string SupplierId { get; private set; } = Guid.NewGuid().ToString();

    public string Id => SupplierId;

    public string Name { get; private set; } = string.Empty;

    public string Country { get; private set; } = string.Empty;

    public string ContactEmail { get; private set; } = string.Empty;

    public string PhoneNumber { get; private set; } = string.Empty;

    public bool IsActive { get; private set; } = true;

    public List<Product> Products { get; private set; } = new();

    private Supplier()
    {
    }

    public Supplier(
        string name,
        string country,
        string contactEmail,
        string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Supplier name is required");

        SupplierId = Guid.NewGuid().ToString();
        Name = name;
        Country = country;
        ContactEmail = contactEmail;
        PhoneNumber = phoneNumber;
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
