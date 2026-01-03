using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace B2Connect.Catalog.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddTaxRatesTableIssue30 : Migration
{
    private static readonly string[] columns = new[] { "id", "country_code", "standard_vat_rate", "reduced_vat_rate", "effective_date", "created_at", "updated_at" };

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "tax_rates",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                country_code = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                standard_vat_rate = table.Column<decimal>(type: "numeric(5,2)", nullable: false, precision: 5, scale: 2),
                reduced_vat_rate = table.Column<decimal>(type: "numeric(5,2)", nullable: true, precision: 5, scale: 2),
                effective_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                expiry_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table => table.PrimaryKey("pk_tax_rates", x => x.id));

        migrationBuilder.CreateIndex(
            name: "ix_tax_rates_country_code",
            table: "tax_rates",
            column: "country_code",
            unique: true);

        // Seed initial tax rates for EU countries
        migrationBuilder.InsertData(
            table: "tax_rates",
            columns: columns,
            values: new object[,]
            {
                { Guid.NewGuid(), "DE", 19.00m, 7.00m, new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                { Guid.NewGuid(), "AT", 20.00m, 10.00m, new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                { Guid.NewGuid(), "FR", 20.00m, 5.50m, new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                { Guid.NewGuid(), "IT", 22.00m, 5.00m, new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                { Guid.NewGuid(), "NL", 21.00m, 9.00m, new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "tax_rates");
    }
}
