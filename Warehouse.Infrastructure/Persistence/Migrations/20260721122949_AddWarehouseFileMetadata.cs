using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Warehouse.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWarehouseFileMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WarehouseFiles",
                columns: table => new
                {
                    FileId = table.Column<string>(type: "text", nullable: false),
                    RelatedEntityId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RelatedEntityType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FileCategory = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    OriginalFileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ObjectKey = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SizeInBytes = table.Column<long>(type: "bigint", nullable: false),
                    UploadedByFirebaseUid = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseFiles", x => x.FileId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseFiles_ObjectKey",
                table: "WarehouseFiles",
                column: "ObjectKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseFiles_RelatedEntityId",
                table: "WarehouseFiles",
                column: "RelatedEntityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WarehouseFiles");
        }
    }
}

