using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupremeCourt.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedByPlayerToWaitingRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "GameId",
                table: "WaitingRooms",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "CreatedByPlayerId",
                table: "WaitingRooms",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByPlayerId",
                table: "WaitingRooms");

            migrationBuilder.AlterColumn<int>(
                name: "GameId",
                table: "WaitingRooms",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
