using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace B2X.CMS.Infrastructure.Migrations;

/// <inheritdoc />
public partial class InitialCmsTemplateOverrides : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "template_overrides",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                template_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                page_definition = table.Column<string>(type: "jsonb", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                is_published = table.Column<bool>(type: "boolean", nullable: false),
                published_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_template_overrides", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "template_override_metadata",
            columns: table => new
            {
                override_id = table.Column<Guid>(type: "uuid", nullable: false),
                version = table.Column<int>(type: "integer", nullable: false),
                validation_status = table.Column<int>(type: "integer", nullable: false),
                is_validated = table.Column<bool>(type: "boolean", nullable: false),
                validation_results = table.Column<string>(type: "jsonb", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_template_override_metadata", x => x.override_id);
                table.ForeignKey(
                    name: "fk_template_override_metadata_template_overrides_override_id",
                    column: x => x.override_id,
                    principalTable: "template_overrides",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        // Indexes for performance and tenant isolation
        migrationBuilder.CreateIndex(
            name: "ix_template_overrides_tenant_id",
            table: "template_overrides",
            column: "tenant_id");

        migrationBuilder.CreateIndex(
            name: "ix_template_overrides_tenant_id_template_name",
            table: "template_overrides",
            columns: new[] { "tenant_id", "template_name" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_template_overrides_tenant_id_is_published",
            table: "template_overrides",
            columns: new[] { "tenant_id", "is_published" });

        migrationBuilder.CreateIndex(
            name: "ix_template_override_metadata_validation_status",
            table: "template_override_metadata",
            column: "validation_status");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "template_override_metadata");

        migrationBuilder.DropTable(
            name: "template_overrides");
    }
}
