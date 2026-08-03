using Warehouse.Domain.Models;

namespace Warehouse.Infrastructure.Data;

public static class FakeWarehouseStore
{
    public static List<Supplier> Suppliers { get; set; } = new()
    {
        new Supplier(
            "Dell Supplier",
            "USA",
            "contact@dell.com",
            "+1 555 111 222"
        ),

        new Supplier(
            "Logitech Supplier",
            "Switzerland",
            "contact@logitech.com",
            "+41 555 333 444"
        ),

        new Supplier(
            "Samsung Supplier",
            "South Korea",
            "contact@samsung.com",
            "+82 555 555 666"
        )
    };

    public static List<Product> Products { get; set; } = new()
    {
        new Product(
            "Laptop",
            "LAP-001",
            "Business laptop",
            1200m,
            15,
            "Dell Supplier",
            DateTime.UtcNow.AddYears(3)
        ),

        new Product(
            "Mouse",
            "MOU-001",
            "Wireless mouse",
            25m,
            100,
            "Logitech Supplier",
            DateTime.UtcNow.AddYears(2)
        ),

        new Product(
            "Keyboard",
            "KEY-001",
            "Mechanical keyboard",
            80m,
            50,
            "Logitech Supplier",
            DateTime.UtcNow.AddYears(2)
        ),

        new Product(
            "Scanner",
            "SCA-001",
            "Office scanner",
            150m,
            20,
            "Samsung Supplier",
            DateTime.UtcNow.AddYears(4)
        ),

        new Product(
            "Printer",
            "PRI-001",
            "Laser printer",
            300m,
            12,
            "Samsung Supplier",
            DateTime.UtcNow.AddYears(4)
        ),

        new Product(
            "Monitor",
            "MON-001",
            "24 inch monitor",
            200m,
            30,
            "Samsung Supplier",
            DateTime.UtcNow.AddYears(4)
        ),

        new Product(
            "USB Cable",
            "USB-001",
            "USB-C cable",
            10m,
            200,
            "Logitech Supplier",
            DateTime.UtcNow.AddYears(2)
        ),

        new Product(
            "Router",
            "ROU-001",
            "Wireless router",
            90m,
            25,
            "Dell Supplier",
            DateTime.UtcNow.AddYears(3)
        ),

        new Product(
            "Hard Drive",
            "HDD-001",
            "External hard drive",
            110m,
            40,
            "Dell Supplier",
            DateTime.UtcNow.AddYears(3)
        ),

        new Product(
            "Webcam",
            "WEB-001",
            "HD webcam",
            60m,
            35,
            "Logitech Supplier",
            DateTime.UtcNow.AddYears(2)
        )
    };

    public static List<ProductImage> ProductImages { get; set; } = new();
}
