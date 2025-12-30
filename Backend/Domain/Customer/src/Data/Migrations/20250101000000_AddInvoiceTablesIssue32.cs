using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace B2Connect.Customer.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddInvoiceTablesIssue32 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "invoice_templates",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    is_default = table.Column<bool>(type: "boolean", nullable: false),
                    company_name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    company_vat_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    company_address = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    company_logo = table.Column<string>(type: "text", nullable: true),
                    company_phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    company_email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    company_website = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    footer_text = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    payment_instructions = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    delivery_notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    tax_note_b2c = table.Column<string>(type: "text", nullable: true),
                    tax_note_b2b = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_invoice_templates", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "invoices",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    invoice_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    issued_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    due_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    seller_name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    seller_vat_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    seller_address = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    buyer_name = table.Column<string>(type: "text", nullable: true),
                    buyer_name_encrypted = table.Column<string>(type: "text", nullable: true),
                    buyer_vat_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    buyer_address = table.Column<string>(type: "text", nullable: true),
                    buyer_address_encrypted = table.Column<string>(type: "text", nullable: true),
                    buyer_country = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    sub_total = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    tax_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    tax_rate = table.Column<decimal>(type: "numeric(5,4)", nullable: false),
                    shipping_cost = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    shipping_tax_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    total = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    reverse_charge_applies = table.Column<bool>(type: "boolean", nullable: false),
                    reverse_charge_note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    payment_method = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    payment_status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    paid_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    pdf_content = table.Column<byte[]>(type: "bytea", nullable: true),
                    pdf_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    pdf_generated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    xml_content = table.Column<string>(type: "text", nullable: true),
                    is_erechnung = table.Column<bool>(type: "boolean", nullable: false),
                    xml_generated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    modified_by = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_invoices", x => x.id);
                    table.ForeignKey(
                        name: "fk_invoices_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "invoice_line_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    invoice_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_sku = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    product_name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    unit_price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    line_sub_total = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    line_tax_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    line_tax_rate = table.Column<decimal>(type: "numeric(5,4)", nullable: false),
                    line_total = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_invoice_line_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_invoice_line_items_invoices_invoice_id",
                        column: x => x.invoice_id,
                        principalTable: "invoices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_invoice_line_items_invoice_id",
                table: "invoice_line_items",
                column: "invoice_id");

            migrationBuilder.CreateIndex(
                name: "ix_invoice_templates_tenant_id_is_default",
                table: "invoice_templates",
                columns: new[] { "tenant_id", "is_default" });

            migrationBuilder.CreateIndex(
                name: "ix_invoices_invoice_number",
                table: "invoices",
                column: "invoice_number");

            migrationBuilder.CreateIndex(
                name: "ix_invoices_issued_at",
                table: "invoices",
                column: "issued_at");

            migrationBuilder.CreateIndex(
                name: "ix_invoices_order_id",
                table: "invoices",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "ix_invoices_tenant_id_id",
                table: "invoices",
                columns: new[] { "tenant_id", "id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "invoice_line_items");

            migrationBuilder.DropTable(
                name: "invoice_templates");

            migrationBuilder.DropTable(
                name: "invoices");
        }
    }
}
