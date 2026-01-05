using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace B2Connect.Localization.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantTranslationsFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "LocalizedStrings",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "LocalizedStrings",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "LocalizedStrings");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "LocalizedStrings");
        }
    }
}
