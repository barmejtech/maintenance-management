using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maintenance_management.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPredictiveMaintenanceAndDigitalTwin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EquipmentDigitalTwins",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EquipmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrentStatus = table.Column<int>(type: "int", nullable: false),
                    WearPercentage = table.Column<double>(type: "float", nullable: false),
                    PerformanceScore = table.Column<double>(type: "float", nullable: false),
                    TemperatureCelsius = table.Column<double>(type: "float", nullable: true),
                    UsageHours = table.Column<double>(type: "float", nullable: true),
                    LastKnownIssue = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    LastSyncedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SimulationNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentDigitalTwins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquipmentDigitalTwins_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentHealthPredictions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EquipmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PredictedFailureDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FailureProbability = table.Column<double>(type: "float", nullable: false),
                    Recommendation = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    TotalInterventions = table.Column<int>(type: "int", nullable: false),
                    AverageDaysBetweenFailures = table.Column<double>(type: "float", nullable: false),
                    AverageDaysBetweenMaintenance = table.Column<double>(type: "float", nullable: false),
                    LastAnalyzedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentHealthPredictions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquipmentHealthPredictions_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentDigitalTwins_EquipmentId",
                table: "EquipmentDigitalTwins",
                column: "EquipmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentHealthPredictions_EquipmentId",
                table: "EquipmentHealthPredictions",
                column: "EquipmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EquipmentDigitalTwins");

            migrationBuilder.DropTable(
                name: "EquipmentHealthPredictions");
        }
    }
}
