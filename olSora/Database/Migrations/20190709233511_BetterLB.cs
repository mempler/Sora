using Microsoft.EntityFrameworkCore.Migrations;

namespace Sora.Database.Migrations
{
    public partial class BetterLB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "PerformancePointsMania",
                table: "LeaderboardRx",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<ulong>(
                name: "PlayCountMania",
                table: "LeaderboardRx",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "RankedScoreMania",
                table: "LeaderboardRx",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "TotalScoreMania",
                table: "LeaderboardRx",
                nullable: false,
                defaultValue: 0ul);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PerformancePointsMania",
                table: "LeaderboardRx");

            migrationBuilder.DropColumn(
                name: "PlayCountMania",
                table: "LeaderboardRx");

            migrationBuilder.DropColumn(
                name: "RankedScoreMania",
                table: "LeaderboardRx");

            migrationBuilder.DropColumn(
                name: "TotalScoreMania",
                table: "LeaderboardRx");
        }
    }
}
