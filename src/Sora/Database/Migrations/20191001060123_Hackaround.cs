using Microsoft.EntityFrameworkCore.Migrations;

namespace Sora.Database.Migrations
{
    public partial class Hackaround : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Leaderboard_Users_Id",
                table: "Leaderboard");

            migrationBuilder.DropColumn(
                name: "ReplayId",
                table: "Scores");

            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "Leaderboard",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Leaderboard_OwnerId",
                table: "Leaderboard",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Leaderboard_Users_OwnerId",
                table: "Leaderboard",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Leaderboard_Users_OwnerId",
                table: "Leaderboard");

            migrationBuilder.DropIndex(
                name: "IX_Leaderboard_OwnerId",
                table: "Leaderboard");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Leaderboard");

            migrationBuilder.AddColumn<int>(
                name: "ReplayId",
                table: "Scores",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Leaderboard_Users_Id",
                table: "Leaderboard",
                column: "Id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
