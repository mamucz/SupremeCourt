using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupremeCourt.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAiFieldsToPlayer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAi",
                table: "Players",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAi",
                table: "Players");
        }
    }
}
