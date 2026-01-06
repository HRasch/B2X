using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace B2Connect.LocalizationService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        private static readonly string[] columns = new[] { "Key", "Category", "TenantId" };

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LocalizedStrings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Translations = table.Column<string>(type: "text", nullable: false),
                    DefaultValue = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table => table.PrimaryKey("PK_LocalizedStrings", x => x.Id));

            migrationBuilder.CreateIndex(
                name: "IX_LocalizedString_Category",
                table: "LocalizedStrings",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_LocalizedString_CreatedAt",
                table: "LocalizedStrings",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_LocalizedString_KeyCategoryTenant",
                table: "LocalizedStrings",
                columns: columns,
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LocalizedString_TenantId",
                table: "LocalizedStrings",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocalizedStrings");
        }
    }
}
