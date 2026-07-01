namespace warehouse_management;
using warehouse_management.Models;

public class FakeWarehouseStore
{
    public static List<Products> DummyProducts = new List<Products>()
    {
        new Products()
        {
            Id = "1dc4d061-07d9-4c36-b2cb-67c1f54fa2ea",
            Name = "Laptop",
            SKU = "LAP-001",
            Description = "Dell business laptop",
            Price = 850,
            QuantityInStock = 15,
            SupplierName = "Dell Supplier",
            ExpiryDate = DateTime.UtcNow.AddYears(3),
            IsArchived = false,
            Created_at = DateTime.UtcNow,
            Last_Updated_at = DateTime.UtcNow
        },
        new Products()
        {
            Id = "72731f45-ba97-4a87-92e3-fd2e2599a473",
            Name = "Mouse",
            SKU = "MOU-001",
            Description = "Wireless mouse",
            Price = 25,
            QuantityInStock = 50,
            SupplierName = "Logitech Supplier",
            ExpiryDate = DateTime.UtcNow.AddYears(2),
            IsArchived = false,
            Created_at = DateTime.UtcNow,
            Last_Updated_at = DateTime.UtcNow
        },
        new Products()
        {
            Id = "4eeb5157-18d5-4a4f-9c98-9c9cdb9a60ab",
            Name = "Keyboard",
            SKU = "KEY-001",
            Description = "Mechanical keyboard",
            Price = 70,
            QuantityInStock = 30,
            SupplierName = "HP Supplier",
            ExpiryDate = DateTime.UtcNow.AddYears(2),
            IsArchived = false,
            Created_at = DateTime.UtcNow,
            Last_Updated_at = DateTime.UtcNow
        },
        new Products()
        {
            Id = "787202b5-46b5-415e-a3f3-b3e61f3eff0c",
            Name = "Scanner",
            SKU = "SCA-001",
            Description = "Office document scanner",
            Price = 180,
            QuantityInStock = 10,
            SupplierName = "Canon Supplier",
            ExpiryDate = DateTime.UtcNow.AddYears(4),
            IsArchived = false,
            Created_at = DateTime.UtcNow,
            Last_Updated_at = DateTime.UtcNow
        },
        new Products()
        {
            Id = "039c7e92-9fae-4804-b06c-d62e7ae0e106",
            Name = "Printer",
            SKU = "PRI-001",
            Description = "Laser printer",
            Price = 220,
            QuantityInStock = 12,
            SupplierName = "Epson Supplier",
            ExpiryDate = DateTime.UtcNow.AddYears(4),
            IsArchived = false,
            Created_at = DateTime.UtcNow,
            Last_Updated_at = DateTime.UtcNow
        },
        new Products()
        {
            Id = "a39d4014-08b3-417e-be76-028a448cc22f",
            Name = "Monitor",
            SKU = "MON-001",
            Description = "24 inch LED monitor",
            Price = 160,
            QuantityInStock = 20,
            SupplierName = "Samsung Supplier",
            ExpiryDate = DateTime.UtcNow.AddYears(5),
            IsArchived = false,
            Created_at = DateTime.UtcNow,
            Last_Updated_at = DateTime.UtcNow
        },

        new Products()
        {
            Id = "eb410f55-30c4-4df7-a2a6-56e34b02449d",
            Name = "Headset",
            SKU = "HEA-001",
            Description = "USB office headset",
            Price = 45,
            QuantityInStock = 25,
            SupplierName = "Jabra Supplier",
            ExpiryDate = DateTime.UtcNow.AddYears(2),
            IsArchived = false,
            Created_at = DateTime.UtcNow,
            Last_Updated_at = DateTime.UtcNow
        },

        new Products()
        {
            Id = "8bfc1e59-879a-4d32-9859-511fc0b1bc36",
            Name = "Webcam",
            SKU = "WEB-001",
            Description = "HD webcam",
            Price = 55,
            QuantityInStock = 18,
            SupplierName = "Logitech Supplier",
            ExpiryDate = DateTime.UtcNow.AddYears(2),
            IsArchived = false,
            Created_at = DateTime.UtcNow,
            Last_Updated_at = DateTime.UtcNow
        },

        new Products()
        {
            Id = "5db39d91-51d7-4b5c-8442-ad862375c305",
            Name = "Router",
            SKU = "ROU-001",
            Description = "Wireless internet router",
            Price = 95,
            QuantityInStock = 14,
            SupplierName = "TP-Link Supplier",
            ExpiryDate = DateTime.UtcNow.AddYears(3),
            IsArchived = false,
            Created_at = DateTime.UtcNow,
            Last_Updated_at = DateTime.UtcNow
        },

        new Products()
        {
            Id = "284243fc-1a8a-4a73-a539-f416f234f8e6",
            Name = "External Hard Drive",
            SKU = "HDD-001",
            Description = "1TB external hard drive",
            Price = 75,
            QuantityInStock = 22,
            SupplierName = "Seagate Supplier",
            ExpiryDate = DateTime.UtcNow.AddYears(3),
            IsArchived = false,
            Created_at = DateTime.UtcNow,
            Last_Updated_at = DateTime.UtcNow
        }
    };

}