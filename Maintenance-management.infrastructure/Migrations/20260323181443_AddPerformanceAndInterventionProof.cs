using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maintenance_management.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceAndInterventionProof : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "ArrivalLatitude",
                table: "TaskOrders",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ArrivalLongitude",
                table: "TaskOrders",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ArrivalTime",
                table: "TaskOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerSignatureUrl",
                table: "TaskOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsGpsValidated",
                table: "TaskOrders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ProofPhotoUrl",
                table: "TaskOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TechnicianPerformanceScores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TechnicianId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AverageInterventionTimeMinutes = table.Column<double>(type: "float", nullable: false),
                    SuccessRate = table.Column<double>(type: "float", nullable: false),
                    CustomerSatisfactionScore = table.Column<double>(type: "float", nullable: false),
                    OnTimeRate = table.Column<double>(type: "float", nullable: false),
                    TotalTasksCompleted = table.Column<int>(type: "int", nullable: false),
                    TotalTasksDelayed = table.Column<int>(type: "int", nullable: false),
                    LastCalculatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicianPerformanceScores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TechnicianPerformanceScores_Technicians_TechnicianId",
                        column: x => x.TechnicianId,
                        principalTable: "Technicians",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TechnicianPerformanceScores_TechnicianId",
                table: "TechnicianPerformanceScores",
                column: "TechnicianId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TechnicianPerformanceScores");

            migrationBuilder.DropColumn(
                name: "ArrivalLatitude",
                table: "TaskOrders");

            migrationBuilder.DropColumn(
                name: "ArrivalLongitude",
                table: "TaskOrders");

            migrationBuilder.DropColumn(
                name: "ArrivalTime",
                table: "TaskOrders");

            migrationBuilder.DropColumn(
                name: "CustomerSignatureUrl",
                table: "TaskOrders");

            migrationBuilder.DropColumn(
                name: "IsGpsValidated",
                table: "TaskOrders");

            migrationBuilder.DropColumn(
                name: "ProofPhotoUrl",
                table: "TaskOrders");
        }
    }
}
