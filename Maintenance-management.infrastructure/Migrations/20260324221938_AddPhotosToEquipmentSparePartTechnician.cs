using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maintenance_management.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPhotosToEquipmentSparePartTechnician : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Photo1Url",
                table: "SpareParts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Photo2Url",
                table: "SpareParts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Photo3Url",
                table: "SpareParts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Photo4Url",
                table: "SpareParts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Photo3Url",
                table: "Equipments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Photo4Url",
                table: "Equipments",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Photo1Url",
                table: "SpareParts");

            migrationBuilder.DropColumn(
                name: "Photo2Url",
                table: "SpareParts");

            migrationBuilder.DropColumn(
                name: "Photo3Url",
                table: "SpareParts");

            migrationBuilder.DropColumn(
                name: "Photo4Url",
                table: "SpareParts");

            migrationBuilder.DropColumn(
                name: "Photo3Url",
                table: "Equipments");

            migrationBuilder.DropColumn(
                name: "Photo4Url",
                table: "Equipments");
        }
    }
}
