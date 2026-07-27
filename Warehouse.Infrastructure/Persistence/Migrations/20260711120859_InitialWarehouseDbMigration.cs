using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Warehouse.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialWarehouseDbMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    SupplierId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ContactEmail = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.SupplierId);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    SKU = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    QuantityInStock = table.Column<int>(type: "integer", nullable: false),
                    SupplierId = table.Column<string>(type: "text", nullable: true),
                    SupplierName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_Products_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ProductImages",
                columns: table => new
                {
                    ProductImageId = table.Column<string>(type: "text", nullable: false),
                    ProductId = table.Column<string>(type: "text", nullable: false),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "text", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImages", x => x.ProductImageId);
                    table.ForeignKey(
                        name: "FK_ProductImages_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Suppliers",
                columns: new[] { "SupplierId", "ContactEmail", "Country", "IsActive", "Name", "PhoneNumber" },
                values: new object[,]
                {
                    { "11111111-1111-1111-1111-111111111111", "contact@dell.com", "USA", true, "Dell Supplier", "+1 555 111 222" },
                    { "22222222-2222-2222-2222-222222222222", "contact@logitech.com", "Switzerland", true, "Logitech Supplier", "+41 555 333 444" },
                    { "33333333-3333-3333-3333-333333333333", "contact@samsung.com", "South Korea", true, "Samsung Supplier", "+82 555 555 666" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "CreatedAt", "Description", "ExpiryDate", "IsArchived", "LastUpdatedAt", "Name", "Price", "QuantityInStock", "SKU", "SupplierId", "SupplierName" },
                values: new object[,]
                {
                    { "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Business laptop", new DateTime(2029, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Laptop", 1200m, 15, "LAP-001", "11111111-1111-1111-1111-111111111111", "Dell Supplier" },
                    { "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Wireless mouse", new DateTime(2028, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Mouse", 25m, 100, "MOU-001", "22222222-2222-2222-2222-222222222222", "Logitech Supplier" },
                    { "cccccccc-cccc-cccc-cccc-cccccccccccc", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Mechanical keyboard", new DateTime(2028, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Keyboard", 80m, 50, "KEY-001", "22222222-2222-2222-2222-222222222222", "Logitech Supplier" },
                    { "dddddddd-dddd-dddd-dddd-dddddddddddd", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Office scanner", new DateTime(2030, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Scanner", 150m, 20, "SCA-001", "33333333-3333-3333-3333-333333333333", "Samsung Supplier" },
                    { "eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Laser printer", new DateTime(2030, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Printer", 300m, 12, "PRI-001", "33333333-3333-3333-3333-333333333333", "Samsung Supplier" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductId",
                table: "ProductImages",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_SKU",
                table: "Products",
                column: "SKU",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_SupplierId",
                table: "Products",
                column: "SupplierId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductImages");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Suppliers");
        }
    }
}

