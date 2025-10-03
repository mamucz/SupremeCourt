using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupremeCourt.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_WaitingRooms_WaitingRoomId",
                table: "Players");

            migrationBuilder.DropTable(
                name: "WaitingRooms");

            migrationBuilder.DropIndex(
                name: "IX_Players_WaitingRoomId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "IsAi",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "WaitingRoomId",
                table: "Players");

            migrationBuilder.RenameColumn(
                name: "Score",
                table: "Players",
                newName: "numberOfLives");

            migrationBuilder.AddColumn<bool>(
                name: "IsAi",
                table: "Users",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "LostGames",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TimeForRound",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TypeName",
                table: "Users",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "WinGames",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAi",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LostGames",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TimeForRound",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TypeName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "WinGames",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "numberOfLives",
                table: "Players",
                newName: "Score");

            migrationBuilder.AddColumn<bool>(
                name: "IsAi",
                table: "Players",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "WaitingRoomId",
                table: "Players",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WaitingRooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    GameId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedByPlayerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaitingRooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WaitingRooms_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Players_WaitingRoomId",
                table: "Players",
                column: "WaitingRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_WaitingRooms_GameId",
                table: "WaitingRooms",
                column: "GameId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_WaitingRooms_WaitingRoomId",
                table: "Players",
                column: "WaitingRoomId",
                principalTable: "WaitingRooms",
                principalColumn: "Id");
        }
    }
}
