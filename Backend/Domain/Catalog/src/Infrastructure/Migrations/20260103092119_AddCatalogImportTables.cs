using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace B2Connect.Catalog.src.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCatalogImportTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CatalogImports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplierId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CatalogId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ImportTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Version = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ProductCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogImports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaxRates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CountryCode = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    CountryName = table.Column<string>(type: "text", nullable: false),
                    StandardVatRate = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    ReducedVatRate = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    EffectiveDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxRates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CatalogProducts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CatalogImportId = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplierAid = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ProductData = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CatalogProducts_CatalogImports_CatalogImportId",
                        column: x => x.CatalogImportId,
                        principalTable: "CatalogImports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "TaxRates",
                columns: new[] { "Id", "CountryCode", "CountryName", "EffectiveDate", "EndDate", "ReducedVatRate", "StandardVatRate" },
                values: new object[,]
                {
                    { new Guid("11440f7d-5bd9-48cf-af40-35d110b7ffee"), "IT", "Italy", new DateTime(2026, 1, 3, 9, 21, 19, 39, DateTimeKind.Utc).AddTicks(1950), null, 10m, 22m },
                    { new Guid("152dbbe7-3a52-49a0-992c-8c8d446c66f9"), "LT", "Lithuania", new DateTime(2026, 1, 3, 9, 21, 19, 39, DateTimeKind.Utc).AddTicks(1960), null, 9m, 21m },
                    { new Guid("191d66e6-40de-4bd1-801d-a349e0a0ab11"), "LU", "Luxembourg", new DateTime(2026, 1, 3, 9, 21, 19, 39, DateTimeKind.Utc).AddTicks(1960), null, 8m, 17m },
                    { new Guid("1be26f51-fbbd-4ba5-a5b8-01e574a5b1a9"), "LV", "Latvia", new DateTime(2026, 1, 3, 9, 21, 19, 39, DateTimeKind.Utc).AddTicks(1950), null, 12m, 21m },
                    { new Guid("28697dc0-079e-4726-9c90-538f387b8239"), "EE", "Estonia", new DateTime(2026, 1, 3, 9, 21, 19, 39, DateTimeKind.Utc).AddTicks(1910), null, 9m, 20m },
                    { new Guid("2a9167e2-ff70-44b8-bd42-bdd78f589c44"), "DE", "Germany", new DateTime(2026, 1, 3, 9, 21, 19, 39, DateTimeKind.Utc).AddTicks(1930), null, 7m, 19m },
                    { new Guid("4d6a2f85-bb50-474d-bbf0-aa0332356c48"), "FI", "Finland", new DateTime(2026, 1, 3, 9, 21, 19, 39, DateTimeKind.Utc).AddTicks(1910), null, 14m, 24m },
                    { new Guid("5727d992-5a6d-4606-ba67-7ed21a318e80"), "IE", "Ireland", new DateTime(2026, 1, 3, 9, 21, 19, 39, DateTimeKind.Utc).AddTicks(1950), null, 13.5m, 23m },
                    { new Guid("5eef59f3-fae0-4acf-9a4a-69069aa78b51"), "MT", "Malta", new DateTime(2026, 1, 3, 9, 21, 19, 39, DateTimeKind.Utc).AddTicks(1960), null, 7m, 18m },
                    { new Guid("6025a514-069d-4648-b3cb-6dbcaa9899ed"), "DK", "Denmark", new DateTime(2026, 1, 3, 9, 21, 19, 39, DateTimeKind.Utc).AddTicks(1910), null, 0m, 25m },
                    { new Guid("644da01e-5156-4f0d-af50-38ec4f31e4b8"), "HU", "Hungary", new DateTime(2026, 1, 3, 9, 21, 19, 39, DateTimeKind.Utc).AddTicks(1940), null, 18m, 27m },
                    { new Guid("67da7019-375d-4f90-9d98-201c82e69668"), "AT", "Austria", new DateTime(2026, 1, 3, 9, 21, 19, 39, DateTimeKind.Utc).AddTicks(1680), null, 10m, 20m },
                    { new Guid("68c539df-be8e-4cee-b2a0-faa5cd92fa66"), "SK", "Slovakia", new DateTime(2026, 1, 3, 9, 21, 19, 39, DateTimeKind.Utc).AddTicks(1980), null, 10m, 20m },
                    { new Guid("7b716df7-178a-4efb-b436-fde1fa2f3e0d"), "BG", "Bulgaria", new DateTime(2026, 1, 3, 9, 21, 19, 39, DateTimeKind.Utc).AddTicks(1890), null, 9m, 20m },
                    { new Guid("8798f28a-7b43-42c3-a708-68b592c84455"), "FR", "France", new DateTime(2026, 1, 3, 9, 21, 19, 39, DateTimeKind.Utc).AddTicks(1930), null, 5.5m, 20m },
                    { new Guid("92119491-36dd-4612-8197-4cee3a1d940d"), "PL", "Poland", new DateTime(2026, 1, 3, 9, 21, 19, 39, DateTimeKind.Utc).AddTicks(1970), null, 8m, 23m },
                    { new Guid("9da79400-087b-435a-add3-ee3107c56e66"), "NL", "Netherlands", new DateTime(2026, 1, 3, 9, 21, 19, 39, DateTimeKind.Utc).AddTicks(1970), null, 9m, 21m },
                    { new Guid("b1c00c89-2180-47ae-bb1d-1c3182a86472"), "RO", "Romania", new DateTime(2026, 1, 3, 9, 21, 19, 39, DateTimeKind.Utc).AddTicks(1980), null, 9m, 19m },
                    { new Guid("ba2ea061-8986-46ec-b3e1-362bb5a51a73"), "SE", "Sweden", new DateTime(2026, 1, 3, 9, 21, 19, 39, DateTimeKind.Utc).AddTicks(1990), null, 12m, 25m },
                    { new Guid("be55eeae-0cc9-432c-8333-91b726cb6579"), "SI", "Slovenia", new DateTime(2026, 1, 3, 9, 21, 19, 39, DateTimeKind.Utc).AddTicks(1990), null, 9.5m, 22m },
                    { new Guid("c1d1e06d-7374-460b-b343-4eeeaa06e2ae"), "ES", "Spain", new DateTime(2026, 1, 3, 9, 21, 19, 39, DateTimeKind.Utc).AddTicks(1990), null, 10m, 21m },
                    { new Guid("c1df6f92-fb89-4d6b-8ce9-b882adfcfc1f"), "GR", "Greece", new DateTime(2026, 1, 3, 9, 21, 19, 39, DateTimeKind.Utc).AddTicks(1940), null, 13m, 24m },
                    { new Guid("c23f9032-60bc-45b4-9057-d295efcb8085"), "CY", "Cyprus", new DateTime(2026, 1, 3, 9, 21, 19, 39, DateTimeKind.Utc).AddTicks(1900), null, 9m, 19m },
                    { new Guid("cf024566-e1e2-4ec4-ada3-aeb8d4280625"), "CZ", "Czech Republic", new DateTime(2026, 1, 3, 9, 21, 19, 39, DateTimeKind.Utc).AddTicks(1900), null, 15m, 21m },
                    { new Guid("eb2d8c4f-a80f-4d67-80e8-e0857e76a970"), "BE", "Belgium", new DateTime(2026, 1, 3, 9, 21, 19, 39, DateTimeKind.Utc).AddTicks(1890), null, 6m, 21m },
                    { new Guid("f0cdc4d5-bd8a-4c3e-a648-47ee70f451ac"), "HR", "Croatia", new DateTime(2026, 1, 3, 9, 21, 19, 39, DateTimeKind.Utc).AddTicks(1890), null, 13m, 25m },
                    { new Guid("fc330729-33b3-486f-8f5f-3b95cacacf6e"), "PT", "Portugal", new DateTime(2026, 1, 3, 9, 21, 19, 39, DateTimeKind.Utc).AddTicks(1980), null, 13m, 23m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CatalogImport_CompositeKey",
                table: "CatalogImports",
                columns: new[] { "TenantId", "SupplierId", "CatalogId", "ImportTimestamp" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CatalogImport_TenantId",
                table: "CatalogImports",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogProduct_CatalogImportId",
                table: "CatalogProducts",
                column: "CatalogImportId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogProduct_ImportId_SupplierAid",
                table: "CatalogProducts",
                columns: new[] { "CatalogImportId", "SupplierAid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaxRate_CountryCode",
                table: "TaxRates",
                column: "CountryCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CatalogProducts");

            migrationBuilder.DropTable(
                name: "TaxRates");

            migrationBuilder.DropTable(
                name: "CatalogImports");
        }
    }
}
