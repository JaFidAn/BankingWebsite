using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditLogExtras : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BrowserInfo",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Endpoint",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IPAddress",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BrowserInfo",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "Endpoint",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "IPAddress",
                table: "AuditLogs");
        }
    }
}
