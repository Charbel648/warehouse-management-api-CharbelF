using Warehouse.Domain.Models;

namespace Warehouse.Api.IntegrationTests.Infrastructure;

public class TestWarehouseStore
{
    public List<Product> Products { get; } = new()
    {
        new Product(
            "Seeded Laptop",
            "SEEDED-LAPTOP",
            "Seeded product for integration tests",
            1200,
            10,
            "Seeded Supplier",
            DateTime.UtcNow.AddYears(1))
    };

    public List<Supplier> Suppliers { get; } = new()
    {
        new Supplier(
            "Seeded Supplier",
            "Lebanon",
            "seeded.supplier@test.com",
            "+96100000000")
    };

    public List<WarehouseFile> Files { get; } = new();
}
