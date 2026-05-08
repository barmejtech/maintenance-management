using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maintenance_management.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVehicleEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MileageAtService",
                table: "TaskOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServiceType",
                table: "TaskOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "VehicleId",
                table: "TaskOrders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MileageInterval",
                table: "MaintenanceSchedules",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "VehicleId",
                table: "MaintenanceSchedules",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "VehicleId",
                table: "Documents",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VIN = table.Column<string>(type: "nvarchar(17)", maxLength: 17, nullable: false),
                    Make = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    LicensePlate = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Color = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Mileage = table.Column<int>(type: "int", nullable: true),
                    EngineType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TransmissionType = table.Column<int>(type: "int", nullable: true),
                    FuelType = table.Column<int>(type: "int", nullable: false),
                    OwnerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    OwnerPhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OwnerEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    PurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastServiceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextServiceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastServiceMileage = table.Column<int>(type: "int", nullable: true),
                    NextServiceMileage = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    VehiclePhotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Photo2Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Photo3Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Photo4Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QrCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskOrders_VehicleId",
                table: "TaskOrders",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceSchedules_VehicleId",
                table: "MaintenanceSchedules",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_VehicleId",
                table: "Documents",
                column: "VehicleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Vehicles_VehicleId",
                table: "Documents",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceSchedules_Vehicles_VehicleId",
                table: "MaintenanceSchedules",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskOrders_Vehicles_VehicleId",
                table: "TaskOrders",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Vehicles_VehicleId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceSchedules_Vehicles_VehicleId",
                table: "MaintenanceSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskOrders_Vehicles_VehicleId",
                table: "TaskOrders");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_TaskOrders_VehicleId",
                table: "TaskOrders");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceSchedules_VehicleId",
                table: "MaintenanceSchedules");

            migrationBuilder.DropIndex(
                name: "IX_Documents_VehicleId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "MileageAtService",
                table: "TaskOrders");

            migrationBuilder.DropColumn(
                name: "ServiceType",
                table: "TaskOrders");

            migrationBuilder.DropColumn(
                name: "VehicleId",
                table: "TaskOrders");

            migrationBuilder.DropColumn(
                name: "MileageInterval",
                table: "MaintenanceSchedules");

            migrationBuilder.DropColumn(
                name: "VehicleId",
                table: "MaintenanceSchedules");

            migrationBuilder.DropColumn(
                name: "VehicleId",
                table: "Documents");
        }
    }
}
