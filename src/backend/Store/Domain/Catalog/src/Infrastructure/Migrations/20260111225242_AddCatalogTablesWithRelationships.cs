using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace B2X.Catalog.src.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCatalogTablesWithRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Categories_ParentId",
                table: "Categories");

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("08915e4d-5e86-4d11-8e94-849e9697db8a"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("16c8ac94-d30c-4a4f-b9a1-f648b08ac557"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("1a17594d-ab08-424a-98ec-abe073d7fcf6"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("1aaa0d1f-c2a0-4c06-b282-22869da12ca9"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("28dc4e6f-9d37-43a7-9dd8-97f81608ac27"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("2952518d-c471-4390-a6f1-49479fb88c2f"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("2d94a169-1c93-42a4-b9bf-f44d9a491919"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("35a39716-a379-447d-a940-fa0dcb60871c"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("37ab6ef3-b244-4028-9a52-9b0e263bc3e8"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("3c693dfc-1239-46d9-8200-62a5c14c4376"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("3decaa1b-0e3c-4f62-92df-1493632af49f"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("47d0e0e4-b78f-427e-bd1c-1e98cff7ccaa"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("5451faeb-eed8-4223-a54e-34afd5f2cd60"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("5e9438f2-b32f-4c03-ba5c-50a480388d4c"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("61732ea8-dd02-40ef-a5fd-48a4b3be7b8c"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("70b3ecaf-ccf8-41f7-a351-ac782c1bae8b"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("7d6c645f-7c32-4cb6-98cc-09e2209b49d0"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("9fe9abbc-2cdb-4d64-bfc9-aaa6c3be4b0b"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("ada31c6e-b171-4bde-9baf-dc659ecb6571"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("b95b3514-1e5e-4fda-94ed-685a80d9f0a0"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("b96e5176-3ebd-4268-9dbf-3c0ff96a3be2"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("c17f0a31-19c0-4ea9-9de9-49e0e7648e44"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("d2f0f094-96d6-442b-9065-406e051b9bc4"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("dc60192a-437a-4ce4-a967-c7651c0b6251"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("e4455ba2-178d-4d6d-8791-2e966319d9a8"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("e53a68a9-9ae3-4127-970b-f76db09327cc"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("f7b03983-6996-4ac5-9c94-51accaa36ff5"));

            migrationBuilder.AlterColumn<string>(
                name: "Weight",
                table: "Variants",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Variants",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "Sku",
                table: "Variants",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "PrimaryImageUrl",
                table: "Variants",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Variants",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Dimensions",
                table: "Variants",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Variants",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Variants",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "Barcode",
                table: "Variants",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Products",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "Sku",
                table: "Products",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Products",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Products",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Products",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "BrandName",
                table: "Products",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Barcode",
                table: "Products",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Categories",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Categories",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Categories",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "MetaTitle",
                table: "Categories",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MetaDescription",
                table: "Categories",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Categories",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Icon",
                table: "Categories",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Categories",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Categories",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.CreateTable(
                name: "ProductCategories",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategories", x => new { x.ProductId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_ProductCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductCategories_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "TaxRates",
                columns: new[] { "Id", "CountryCode", "CountryName", "EffectiveDate", "EndDate", "ReducedVatRate", "StandardVatRate" },
                values: new object[,]
                {
                    { new Guid("00ac79c0-23c8-4521-a120-f99788644961"), "FR", "France", new DateTime(2026, 1, 11, 22, 52, 42, 99, DateTimeKind.Utc).AddTicks(3873), null, 5.5m, 20m },
                    { new Guid("02936643-30b5-4539-9e45-6d63d25babfc"), "HR", "Croatia", new DateTime(2026, 1, 11, 22, 52, 42, 99, DateTimeKind.Utc).AddTicks(3821), null, 13m, 25m },
                    { new Guid("0cf4be29-3179-471c-947b-6ecfe536db65"), "SE", "Sweden", new DateTime(2026, 1, 11, 22, 52, 42, 99, DateTimeKind.Utc).AddTicks(3911), null, 12m, 25m },
                    { new Guid("260e5b59-5fd3-45dc-a008-abd9cafd513e"), "IE", "Ireland", new DateTime(2026, 1, 11, 22, 52, 42, 99, DateTimeKind.Utc).AddTicks(3881), null, 13.5m, 23m },
                    { new Guid("33db00b1-97df-4021-b2c6-9f734c8f04b3"), "LT", "Lithuania", new DateTime(2026, 1, 11, 22, 52, 42, 99, DateTimeKind.Utc).AddTicks(3896), null, 9m, 21m },
                    { new Guid("42da6a0f-80a7-4e4e-a585-e897a850359d"), "LU", "Luxembourg", new DateTime(2026, 1, 11, 22, 52, 42, 99, DateTimeKind.Utc).AddTicks(3897), null, 8m, 17m },
                    { new Guid("4d31726b-af8e-4c8d-8cdb-41b9d8875f72"), "BE", "Belgium", new DateTime(2026, 1, 11, 22, 52, 42, 99, DateTimeKind.Utc).AddTicks(3817), null, 6m, 21m },
                    { new Guid("576a203d-755b-43e2-ab1d-a2ae41f8ae7e"), "SK", "Slovakia", new DateTime(2026, 1, 11, 22, 52, 42, 99, DateTimeKind.Utc).AddTicks(3907), null, 10m, 20m },
                    { new Guid("57a3749c-9e1c-43b1-b31c-2bbe5ea7d5b0"), "LV", "Latvia", new DateTime(2026, 1, 11, 22, 52, 42, 99, DateTimeKind.Utc).AddTicks(3894), null, 12m, 21m },
                    { new Guid("6047c81c-8037-45fd-934e-0857f2211bfa"), "DE", "Germany", new DateTime(2026, 1, 11, 22, 52, 42, 99, DateTimeKind.Utc).AddTicks(3875), null, 7m, 19m },
                    { new Guid("6bb3c3a4-a770-44e4-a67f-47215d2fb23f"), "AT", "Austria", new DateTime(2026, 1, 11, 22, 52, 42, 99, DateTimeKind.Utc).AddTicks(3600), null, 10m, 20m },
                    { new Guid("6fa777eb-3841-4611-a1be-a21aad16001e"), "PT", "Portugal", new DateTime(2026, 1, 11, 22, 52, 42, 99, DateTimeKind.Utc).AddTicks(3904), null, 13m, 23m },
                    { new Guid("7223b111-6d43-4e9f-82b8-ebd5aac92910"), "GR", "Greece", new DateTime(2026, 1, 11, 22, 52, 42, 99, DateTimeKind.Utc).AddTicks(3876), null, 13m, 24m },
                    { new Guid("76f8bcc9-181b-49cd-8646-338602f828ef"), "ES", "Spain", new DateTime(2026, 1, 11, 22, 52, 42, 99, DateTimeKind.Utc).AddTicks(3910), null, 10m, 21m },
                    { new Guid("80a8fd57-dcdc-41d8-9c67-b0d418e5b790"), "MT", "Malta", new DateTime(2026, 1, 11, 22, 52, 42, 99, DateTimeKind.Utc).AddTicks(3899), null, 7m, 18m },
                    { new Guid("869cb317-4f1a-483d-ae9d-14072a304c0f"), "IT", "Italy", new DateTime(2026, 1, 11, 22, 52, 42, 99, DateTimeKind.Utc).AddTicks(3892), null, 10m, 22m },
                    { new Guid("8d8a0b4b-e297-4611-8747-2328e04c3f4d"), "EE", "Estonia", new DateTime(2026, 1, 11, 22, 52, 42, 99, DateTimeKind.Utc).AddTicks(3848), null, 9m, 20m },
                    { new Guid("a1e971dc-462e-42ea-b610-cf78fe51d1fd"), "HU", "Hungary", new DateTime(2026, 1, 11, 22, 52, 42, 99, DateTimeKind.Utc).AddTicks(3879), null, 18m, 27m },
                    { new Guid("aa0badc9-ec43-4a2a-893f-ace50090126f"), "CZ", "Czech Republic", new DateTime(2026, 1, 11, 22, 52, 42, 99, DateTimeKind.Utc).AddTicks(3843), null, 15m, 21m },
                    { new Guid("b479a08b-663e-42f3-82cc-94d48293a702"), "BG", "Bulgaria", new DateTime(2026, 1, 11, 22, 52, 42, 99, DateTimeKind.Utc).AddTicks(3819), null, 9m, 20m },
                    { new Guid("b52bd8d8-7151-4dc6-b774-fb988572434c"), "NL", "Netherlands", new DateTime(2026, 1, 11, 22, 52, 42, 99, DateTimeKind.Utc).AddTicks(3900), null, 9m, 21m },
                    { new Guid("b82f0e11-c388-4f7e-86cd-1ec743f767f4"), "CY", "Cyprus", new DateTime(2026, 1, 11, 22, 52, 42, 99, DateTimeKind.Utc).AddTicks(3830), null, 9m, 19m },
                    { new Guid("b877f898-83b2-4d75-84bc-597f7d7333a3"), "FI", "Finland", new DateTime(2026, 1, 11, 22, 52, 42, 99, DateTimeKind.Utc).AddTicks(3850), null, 14m, 24m },
                    { new Guid("bd29fe8c-4e58-4eb5-b892-88a7e75a46c7"), "PL", "Poland", new DateTime(2026, 1, 11, 22, 52, 42, 99, DateTimeKind.Utc).AddTicks(3903), null, 8m, 23m },
                    { new Guid("cc159c01-21a3-4513-b799-1128fd879780"), "DK", "Denmark", new DateTime(2026, 1, 11, 22, 52, 42, 99, DateTimeKind.Utc).AddTicks(3847), null, 0m, 25m },
                    { new Guid("e97607b0-63cd-456a-9c4f-db24052a0e19"), "RO", "Romania", new DateTime(2026, 1, 11, 22, 52, 42, 99, DateTimeKind.Utc).AddTicks(3906), null, 9m, 19m },
                    { new Guid("f0293508-b564-4459-b1e1-8df645df0a84"), "SI", "Slovenia", new DateTime(2026, 1, 11, 22, 52, 42, 99, DateTimeKind.Utc).AddTicks(3908), null, 9.5m, 22m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Variants_ProductId",
                table: "Variants",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Variants_ProductId_IsActive",
                table: "Variants",
                columns: new[] { "ProductId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Variants_ProductId_Sku",
                table: "Variants",
                columns: new[] { "ProductId", "Sku" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_CreatedAt",
                table: "Products",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Products_TenantId",
                table: "Products",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_TenantId_IsActive",
                table: "Products",
                columns: new[] { "TenantId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Products_TenantId_Sku",
                table: "Products",
                columns: new[] { "TenantId", "Sku" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_TenantId",
                table: "Categories",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_TenantId_Active_Visible",
                table: "Categories",
                columns: new[] { "TenantId", "IsActive", "IsVisible" });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_TenantId_Slug",
                table: "Categories",
                columns: new[] { "TenantId", "Slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_CategoryId",
                table: "ProductCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_ProductId",
                table: "ProductCategories",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Categories_ParentId",
                table: "Categories",
                column: "ParentId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Variants_Products_ProductId",
                table: "Variants",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Categories_ParentId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Variants_Products_ProductId",
                table: "Variants");

            migrationBuilder.DropTable(
                name: "ProductCategories");

            migrationBuilder.DropIndex(
                name: "IX_Variants_ProductId",
                table: "Variants");

            migrationBuilder.DropIndex(
                name: "IX_Variants_ProductId_IsActive",
                table: "Variants");

            migrationBuilder.DropIndex(
                name: "IX_Variants_ProductId_Sku",
                table: "Variants");

            migrationBuilder.DropIndex(
                name: "IX_Products_CreatedAt",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_TenantId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_TenantId_IsActive",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_TenantId_Sku",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Categories_TenantId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_TenantId_Active_Visible",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_TenantId_Slug",
                table: "Categories");

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("00ac79c0-23c8-4521-a120-f99788644961"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("02936643-30b5-4539-9e45-6d63d25babfc"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("0cf4be29-3179-471c-947b-6ecfe536db65"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("260e5b59-5fd3-45dc-a008-abd9cafd513e"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("33db00b1-97df-4021-b2c6-9f734c8f04b3"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("42da6a0f-80a7-4e4e-a585-e897a850359d"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("4d31726b-af8e-4c8d-8cdb-41b9d8875f72"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("576a203d-755b-43e2-ab1d-a2ae41f8ae7e"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("57a3749c-9e1c-43b1-b31c-2bbe5ea7d5b0"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("6047c81c-8037-45fd-934e-0857f2211bfa"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("6bb3c3a4-a770-44e4-a67f-47215d2fb23f"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("6fa777eb-3841-4611-a1be-a21aad16001e"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("7223b111-6d43-4e9f-82b8-ebd5aac92910"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("76f8bcc9-181b-49cd-8646-338602f828ef"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("80a8fd57-dcdc-41d8-9c67-b0d418e5b790"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("869cb317-4f1a-483d-ae9d-14072a304c0f"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("8d8a0b4b-e297-4611-8747-2328e04c3f4d"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("a1e971dc-462e-42ea-b610-cf78fe51d1fd"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("aa0badc9-ec43-4a2a-893f-ace50090126f"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("b479a08b-663e-42f3-82cc-94d48293a702"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("b52bd8d8-7151-4dc6-b774-fb988572434c"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("b82f0e11-c388-4f7e-86cd-1ec743f767f4"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("b877f898-83b2-4d75-84bc-597f7d7333a3"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("bd29fe8c-4e58-4eb5-b892-88a7e75a46c7"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("cc159c01-21a3-4513-b799-1128fd879780"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("e97607b0-63cd-456a-9c4f-db24052a0e19"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("f0293508-b564-4459-b1e1-8df645df0a84"));

            migrationBuilder.AlterColumn<string>(
                name: "Weight",
                table: "Variants",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Variants",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<string>(
                name: "Sku",
                table: "Variants",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "PrimaryImageUrl",
                table: "Variants",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Variants",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Dimensions",
                table: "Variants",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Variants",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Variants",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<string>(
                name: "Barcode",
                table: "Variants",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Products",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<string>(
                name: "Sku",
                table: "Products",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Products",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Products",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Products",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<string>(
                name: "BrandName",
                table: "Products",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Barcode",
                table: "Products",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Categories",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Categories",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Categories",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "MetaTitle",
                table: "Categories",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MetaDescription",
                table: "Categories",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Categories",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Icon",
                table: "Categories",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Categories",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Categories",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.InsertData(
                table: "TaxRates",
                columns: new[] { "Id", "CountryCode", "CountryName", "EffectiveDate", "EndDate", "ReducedVatRate", "StandardVatRate" },
                values: new object[,]
                {
                    { new Guid("08915e4d-5e86-4d11-8e94-849e9697db8a"), "HU", "Hungary", new DateTime(2026, 1, 11, 22, 51, 1, 304, DateTimeKind.Utc).AddTicks(1309), null, 18m, 27m },
                    { new Guid("16c8ac94-d30c-4a4f-b9a1-f648b08ac557"), "BG", "Bulgaria", new DateTime(2026, 1, 11, 22, 51, 1, 304, DateTimeKind.Utc).AddTicks(1242), null, 9m, 20m },
                    { new Guid("1a17594d-ab08-424a-98ec-abe073d7fcf6"), "LT", "Lithuania", new DateTime(2026, 1, 11, 22, 51, 1, 304, DateTimeKind.Utc).AddTicks(1327), null, 9m, 21m },
                    { new Guid("1aaa0d1f-c2a0-4c06-b282-22869da12ca9"), "BE", "Belgium", new DateTime(2026, 1, 11, 22, 51, 1, 304, DateTimeKind.Utc).AddTicks(1240), null, 6m, 21m },
                    { new Guid("28dc4e6f-9d37-43a7-9dd8-97f81608ac27"), "SI", "Slovenia", new DateTime(2026, 1, 11, 22, 51, 1, 304, DateTimeKind.Utc).AddTicks(1340), null, 9.5m, 22m },
                    { new Guid("2952518d-c471-4390-a6f1-49479fb88c2f"), "SE", "Sweden", new DateTime(2026, 1, 11, 22, 51, 1, 304, DateTimeKind.Utc).AddTicks(1343), null, 12m, 25m },
                    { new Guid("2d94a169-1c93-42a4-b9bf-f44d9a491919"), "FI", "Finland", new DateTime(2026, 1, 11, 22, 51, 1, 304, DateTimeKind.Utc).AddTicks(1283), null, 14m, 24m },
                    { new Guid("35a39716-a379-447d-a940-fa0dcb60871c"), "FR", "France", new DateTime(2026, 1, 11, 22, 51, 1, 304, DateTimeKind.Utc).AddTicks(1304), null, 5.5m, 20m },
                    { new Guid("37ab6ef3-b244-4028-9a52-9b0e263bc3e8"), "RO", "Romania", new DateTime(2026, 1, 11, 22, 51, 1, 304, DateTimeKind.Utc).AddTicks(1337), null, 9m, 19m },
                    { new Guid("3c693dfc-1239-46d9-8200-62a5c14c4376"), "EE", "Estonia", new DateTime(2026, 1, 11, 22, 51, 1, 304, DateTimeKind.Utc).AddTicks(1281), null, 9m, 20m },
                    { new Guid("3decaa1b-0e3c-4f62-92df-1493632af49f"), "PT", "Portugal", new DateTime(2026, 1, 11, 22, 51, 1, 304, DateTimeKind.Utc).AddTicks(1334), null, 13m, 23m },
                    { new Guid("47d0e0e4-b78f-427e-bd1c-1e98cff7ccaa"), "DK", "Denmark", new DateTime(2026, 1, 11, 22, 51, 1, 304, DateTimeKind.Utc).AddTicks(1279), null, 0m, 25m },
                    { new Guid("5451faeb-eed8-4223-a54e-34afd5f2cd60"), "AT", "Austria", new DateTime(2026, 1, 11, 22, 51, 1, 304, DateTimeKind.Utc).AddTicks(1026), null, 10m, 20m },
                    { new Guid("5e9438f2-b32f-4c03-ba5c-50a480388d4c"), "NL", "Netherlands", new DateTime(2026, 1, 11, 22, 51, 1, 304, DateTimeKind.Utc).AddTicks(1331), null, 9m, 21m },
                    { new Guid("61732ea8-dd02-40ef-a5fd-48a4b3be7b8c"), "LV", "Latvia", new DateTime(2026, 1, 11, 22, 51, 1, 304, DateTimeKind.Utc).AddTicks(1325), null, 12m, 21m },
                    { new Guid("70b3ecaf-ccf8-41f7-a351-ac782c1bae8b"), "MT", "Malta", new DateTime(2026, 1, 11, 22, 51, 1, 304, DateTimeKind.Utc).AddTicks(1330), null, 7m, 18m },
                    { new Guid("7d6c645f-7c32-4cb6-98cc-09e2209b49d0"), "SK", "Slovakia", new DateTime(2026, 1, 11, 22, 51, 1, 304, DateTimeKind.Utc).AddTicks(1339), null, 10m, 20m },
                    { new Guid("9fe9abbc-2cdb-4d64-bfc9-aaa6c3be4b0b"), "PL", "Poland", new DateTime(2026, 1, 11, 22, 51, 1, 304, DateTimeKind.Utc).AddTicks(1333), null, 8m, 23m },
                    { new Guid("ada31c6e-b171-4bde-9baf-dc659ecb6571"), "DE", "Germany", new DateTime(2026, 1, 11, 22, 51, 1, 304, DateTimeKind.Utc).AddTicks(1306), null, 7m, 19m },
                    { new Guid("b95b3514-1e5e-4fda-94ed-685a80d9f0a0"), "GR", "Greece", new DateTime(2026, 1, 11, 22, 51, 1, 304, DateTimeKind.Utc).AddTicks(1308), null, 13m, 24m },
                    { new Guid("b96e5176-3ebd-4268-9dbf-3c0ff96a3be2"), "IT", "Italy", new DateTime(2026, 1, 11, 22, 51, 1, 304, DateTimeKind.Utc).AddTicks(1314), null, 10m, 22m },
                    { new Guid("c17f0a31-19c0-4ea9-9de9-49e0e7648e44"), "HR", "Croatia", new DateTime(2026, 1, 11, 22, 51, 1, 304, DateTimeKind.Utc).AddTicks(1243), null, 13m, 25m },
                    { new Guid("d2f0f094-96d6-442b-9065-406e051b9bc4"), "CY", "Cyprus", new DateTime(2026, 1, 11, 22, 51, 1, 304, DateTimeKind.Utc).AddTicks(1245), null, 9m, 19m },
                    { new Guid("dc60192a-437a-4ce4-a967-c7651c0b6251"), "LU", "Luxembourg", new DateTime(2026, 1, 11, 22, 51, 1, 304, DateTimeKind.Utc).AddTicks(1328), null, 8m, 17m },
                    { new Guid("e4455ba2-178d-4d6d-8791-2e966319d9a8"), "IE", "Ireland", new DateTime(2026, 1, 11, 22, 51, 1, 304, DateTimeKind.Utc).AddTicks(1311), null, 13.5m, 23m },
                    { new Guid("e53a68a9-9ae3-4127-970b-f76db09327cc"), "ES", "Spain", new DateTime(2026, 1, 11, 22, 51, 1, 304, DateTimeKind.Utc).AddTicks(1341), null, 10m, 21m },
                    { new Guid("f7b03983-6996-4ac5-9c94-51accaa36ff5"), "CZ", "Czech Republic", new DateTime(2026, 1, 11, 22, 51, 1, 304, DateTimeKind.Utc).AddTicks(1254), null, 15m, 21m }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Categories_ParentId",
                table: "Categories",
                column: "ParentId",
                principalTable: "Categories",
                principalColumn: "Id");
        }
    }
}
