using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maintenance_management.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddQrCodeToEquipmentAndSparePart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QrCode",
                table: "SpareParts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QrCode",
                table: "Equipments",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QrCode",
                table: "SpareParts");

            migrationBuilder.DropColumn(
                name: "QrCode",
                table: "Equipments");
        }
    }
}
