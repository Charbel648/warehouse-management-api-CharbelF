using Warehouse.Domain.Models;

namespace Warehouse.Api.UnitTests.Builders;

public class SupplierBuilder
{
    private string _name = "Default Supplier";
    private string _country = "Lebanon";
    private string _contactEmail = "supplier@test.com";
    private string _phoneNumber = "+96100000000";

    public SupplierBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public SupplierBuilder WithCountry(string country)
    {
        _country = country;
        return this;
    }

    public SupplierBuilder WithContactEmail(string contactEmail)
    {
        _contactEmail = contactEmail;
        return this;
    }

    public SupplierBuilder WithPhoneNumber(string phoneNumber)
    {
        _phoneNumber = phoneNumber;
        return this;
    }

    public Supplier Build()
    {
        return new Supplier(
            _name,
            _country,
            _contactEmail,
            _phoneNumber
        );
    }
}
