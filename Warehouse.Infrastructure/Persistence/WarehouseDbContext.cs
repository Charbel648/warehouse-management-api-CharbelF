using Microsoft.EntityFrameworkCore;
using Warehouse.Domain.Models;

namespace Warehouse.Infrastructure.Persistence;

public class WarehouseDbContext : DbContext
{
    public WarehouseDbContext(DbContextOptions<WarehouseDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();

    public DbSet<Supplier> Suppliers => Set<Supplier>();

    public DbSet<ProductImage> ProductImages => Set<ProductImage>();

    
    public DbSet<WarehouseFile> WarehouseFiles => Set<WarehouseFile>();
protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.ToTable("Suppliers");

            entity.HasKey(s => s.SupplierId);

            entity.Property(s => s.SupplierId)
                .IsRequired();

            entity.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(s => s.Country)
                .HasMaxLength(100);

            entity.Property(s => s.ContactEmail)
                .HasMaxLength(150);

            entity.Property(s => s.PhoneNumber)
                .HasMaxLength(50);

            entity.Property(s => s.IsActive)
                .IsRequired();

            entity.Ignore(s => s.Id);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products");

            entity.HasKey(p => p.ProductId);

            entity.Property(p => p.ProductId)
                .IsRequired();

            entity.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(p => p.SKU)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(p => p.SKU)
                .IsUnique();

            entity.Property(p => p.Description)
                .HasMaxLength(1000);

            entity.Property(p => p.Price)
                .HasColumnType("numeric(18,2)");

            entity.Property(p => p.QuantityInStock)
                .IsRequired();

            entity.Property(p => p.SupplierName)
                .HasMaxLength(150);

            entity.Property(p => p.ExpiryDate)
                .IsRequired();

            entity.Property(p => p.IsArchived)
                .IsRequired();

            entity.Property(p => p.CreatedAt)
                .IsRequired();

            entity.Property(p => p.LastUpdatedAt)
                .IsRequired();

            entity.Ignore(p => p.Id);

            entity.HasOne(p => p.Supplier)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.SupplierId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.ToTable("ProductImages");

            entity.HasKey(i => i.ProductImageId);

            entity.Property(i => i.ProductImageId)
                .IsRequired();

            entity.Property(i => i.ProductId)
                .IsRequired();

            entity.Property(i => i.FileName)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(i => i.FilePath)
                .IsRequired();

            entity.Property(i => i.UploadedAt)
                .IsRequired();

            entity.HasOne(i => i.Product)
                .WithMany(p => p.Images)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });


        modelBuilder.Entity<WarehouseFile>(entity =>
        {
            entity.ToTable("WarehouseFiles");

            entity.HasKey(file => file.FileId);

            entity.Property(file => file.FileId)
                .IsRequired();

            entity.Property(file => file.RelatedEntityId)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(file => file.RelatedEntityType)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(file => file.FileCategory)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(file => file.OriginalFileName)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(file => file.ObjectKey)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(file => file.ContentType)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(file => file.SizeInBytes)
                .IsRequired();

            entity.Property(file => file.UploadedByFirebaseUid)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(file => file.UploadedAt)
                .IsRequired();

            entity.HasIndex(file => file.RelatedEntityId);
            entity.HasIndex(file => file.ObjectKey)
                .IsUnique();
        });
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        string dellSupplierId = "11111111-1111-1111-1111-111111111111";
        string logitechSupplierId = "22222222-2222-2222-2222-222222222222";
        string samsungSupplierId = "33333333-3333-3333-3333-333333333333";

        DateTime createdAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<Supplier>().HasData(
            new
            {
                SupplierId = dellSupplierId,
                Name = "Dell Supplier",
                Country = "USA",
                ContactEmail = "contact@dell.com",
                PhoneNumber = "+1 555 111 222",
                IsActive = true
            },
            new
            {
                SupplierId = logitechSupplierId,
                Name = "Logitech Supplier",
                Country = "Switzerland",
                ContactEmail = "contact@logitech.com",
                PhoneNumber = "+41 555 333 444",
                IsActive = true
            },
            new
            {
                SupplierId = samsungSupplierId,
                Name = "Samsung Supplier",
                Country = "South Korea",
                ContactEmail = "contact@samsung.com",
                PhoneNumber = "+82 555 555 666",
                IsActive = true
            }
        );

        modelBuilder.Entity<Product>().HasData(
            new
            {
                ProductId = "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
                Name = "Laptop",
                SKU = "LAP-001",
                Description = "Business laptop",
                Price = 1200m,
                QuantityInStock = 15,
                SupplierId = dellSupplierId,
                SupplierName = "Dell Supplier",
                ExpiryDate = new DateTime(2029, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsArchived = false,
                CreatedAt = createdAt,
                LastUpdatedAt = createdAt
            },
            new
            {
                ProductId = "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb",
                Name = "Mouse",
                SKU = "MOU-001",
                Description = "Wireless mouse",
                Price = 25m,
                QuantityInStock = 100,
                SupplierId = logitechSupplierId,
                SupplierName = "Logitech Supplier",
                ExpiryDate = new DateTime(2028, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsArchived = false,
                CreatedAt = createdAt,
                LastUpdatedAt = createdAt
            },
            new
            {
                ProductId = "cccccccc-cccc-cccc-cccc-cccccccccccc",
                Name = "Keyboard",
                SKU = "KEY-001",
                Description = "Mechanical keyboard",
                Price = 80m,
                QuantityInStock = 50,
                SupplierId = logitechSupplierId,
                SupplierName = "Logitech Supplier",
                ExpiryDate = new DateTime(2028, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsArchived = false,
                CreatedAt = createdAt,
                LastUpdatedAt = createdAt
            },
            new
            {
                ProductId = "dddddddd-dddd-dddd-dddd-dddddddddddd",
                Name = "Scanner",
                SKU = "SCA-001",
                Description = "Office scanner",
                Price = 150m,
                QuantityInStock = 20,
                SupplierId = samsungSupplierId,
                SupplierName = "Samsung Supplier",
                ExpiryDate = new DateTime(2030, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsArchived = false,
                CreatedAt = createdAt,
                LastUpdatedAt = createdAt
            },
            new
            {
                ProductId = "eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee",
                Name = "Printer",
                SKU = "PRI-001",
                Description = "Laser printer",
                Price = 300m,
                QuantityInStock = 12,
                SupplierId = samsungSupplierId,
                SupplierName = "Samsung Supplier",
                ExpiryDate = new DateTime(2030, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsArchived = false,
                CreatedAt = createdAt,
                LastUpdatedAt = createdAt
            }
        );
    }
}


