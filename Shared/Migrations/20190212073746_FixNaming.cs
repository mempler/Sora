using Microsoft.EntityFrameworkCore.Migrations;

namespace Shared.Migrations
{
    public partial class FixNaming : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Count100Mania",
                table: "LeaderboardRx");

            migrationBuilder.DropColumn(
                name: "Count300Mania",
                table: "LeaderboardRx");

            migrationBuilder.DropColumn(
                name: "Count50Mania",
                table: "LeaderboardRx");

            migrationBuilder.DropColumn(
                name: "CountMissMania",
                table: "LeaderboardRx");

            migrationBuilder.DropColumn(
                name: "PerformancePerformance",
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "Count100Mania",
                table: "LeaderboardRx",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "Count300Mania",
                table: "LeaderboardRx",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "Count50Mania",
                table: "LeaderboardRx",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "CountMissMania",
                table: "LeaderboardRx",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<double>(
                name: "PerformancePerformance",
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
    }
}
