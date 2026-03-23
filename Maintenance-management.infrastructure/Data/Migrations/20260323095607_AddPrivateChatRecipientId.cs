using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maintenance_management.infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPrivateChatRecipientId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RecipientId",
                table: "ChatMessages",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecipientId",
                table: "ChatMessages");
        }
    }
}
