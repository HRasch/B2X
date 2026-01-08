using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace B2X.Email.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddEmailEventsTable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "email_events",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                email_id = table.Column<Guid>(type: "uuid", nullable: false),
                tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                event_type = table.Column<int>(type: "integer", nullable: false),
                metadata = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                occurred_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                user_agent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                ip_address = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_email_events", x => x.id);
            });

        // Indexes for performance
        migrationBuilder.CreateIndex(
            name: "ix_email_events_email_id",
            table: "email_events",
            column: "email_id");

        migrationBuilder.CreateIndex(
            name: "ix_email_events_tenant_id",
            table: "email_events",
            column: "tenant_id");

        migrationBuilder.CreateIndex(
            name: "ix_email_events_tenant_id_occurred_at",
            table: "email_events",
            columns: new[] { "tenant_id", "occurred_at" });

        migrationBuilder.CreateIndex(
            name: "ix_email_events_tenant_id_event_type_occurred_at",
            table: "email_events",
            columns: new[] { "tenant_id", "event_type", "occurred_at" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "email_events");
    }
}
