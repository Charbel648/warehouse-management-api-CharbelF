namespace Warehouse.Domain.Models;

public class Supplier
{
    public string Id { get; private set; } = Guid.NewGuid().ToString();

    public string Name { get; private set; }

    public string Country { get; private set; }

    public string ContactEmail { get; private set; }

    public string PhoneNumber { get; private set; }

    public bool IsActive { get; private set; } = true;

    public Supplier(
        string name,
        string country,
        string contactEmail,
        string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Supplier name is required");

        Name = name;
        Country = country;
        ContactEmail = contactEmail;
        PhoneNumber = phoneNumber;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
