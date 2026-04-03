using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maintenance_management.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInvoiceMaintenanceReportLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MaintenanceReportId",
                table: "Invoices",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_MaintenanceReportId",
                table: "Invoices",
                column: "MaintenanceReportId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_MaintenanceReports_MaintenanceReportId",
                table: "Invoices",
                column: "MaintenanceReportId",
                principalTable: "MaintenanceReports",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_MaintenanceReports_MaintenanceReportId",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_MaintenanceReportId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "MaintenanceReportId",
                table: "Invoices");
        }
    }
}
