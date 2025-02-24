using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupremeCourt.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWaitingRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "WaitingRoomId",
                table: "Players");
        }
    }
}
