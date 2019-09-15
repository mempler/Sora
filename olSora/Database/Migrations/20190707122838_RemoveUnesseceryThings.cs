using Microsoft.EntityFrameworkCore.Migrations;

namespace Sora.Database.Migrations
{
    public partial class RemoveUnesseceryThings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Count100Ctb",
                table: "LeaderboardStd");

            migrationBuilder.DropColumn(
                name: "Count100Mania",
                table: "LeaderboardStd");

            migrationBuilder.DropColumn(
                name: "Count100Osu",
                table: "LeaderboardStd");

            migrationBuilder.DropColumn(
                name: "Count100Taiko",
                table: "LeaderboardStd");

            migrationBuilder.DropColumn(
                name: "Count300Ctb",
                table: "LeaderboardStd");

            migrationBuilder.DropColumn(
                name: "Count300Mania",
                table: "LeaderboardStd");

            migrationBuilder.DropColumn(
                name: "Count300Osu",
                table: "LeaderboardStd");

            migrationBuilder.DropColumn(
                name: "Count300Taiko",
                table: "LeaderboardStd");

            migrationBuilder.DropColumn(
                name: "Count50Ctb",
                table: "LeaderboardStd");

            migrationBuilder.DropColumn(
                name: "Count50Mania",
                table: "LeaderboardStd");

            migrationBuilder.DropColumn(
                name: "Count50Osu",
                table: "LeaderboardStd");

            migrationBuilder.DropColumn(
                name: "Count50Taiko",
                table: "LeaderboardStd");

            migrationBuilder.DropColumn(
                name: "CountMissCtb",
                table: "LeaderboardStd");

            migrationBuilder.DropColumn(
                name: "CountMissMania",
                table: "LeaderboardStd");

            migrationBuilder.DropColumn(
                name: "CountMissOsu",
                table: "LeaderboardStd");

            migrationBuilder.DropColumn(
                name: "CountMissTaiko",
                table: "LeaderboardStd");

            migrationBuilder.DropColumn(
                name: "Count100Ctb",
                table: "LeaderboardRx");

            migrationBuilder.DropColumn(
                name: "Count100Osu",
                table: "LeaderboardRx");

            migrationBuilder.DropColumn(
                name: "Count100Taiko",
                table: "LeaderboardRx");

            migrationBuilder.DropColumn(
                name: "Count300Ctb",
                table: "LeaderboardRx");

            migrationBuilder.DropColumn(
                name: "Count300Osu",
                table: "LeaderboardRx");

            migrationBuilder.DropColumn(
                name: "Count300Taiko",
                table: "LeaderboardRx");

            migrationBuilder.DropColumn(
                name: "Count50Ctb",
                table: "LeaderboardRx");

            migrationBuilder.DropColumn(
                name: "Count50Osu",
                table: "LeaderboardRx");

            migrationBuilder.DropColumn(
                name: "Count50Taiko",
                table: "LeaderboardRx");

            migrationBuilder.DropColumn(
                name: "CountMissCtb",
                table: "LeaderboardRx");

            migrationBuilder.DropColumn(
                name: "CountMissOsu",
                table: "LeaderboardRx");

            migrationBuilder.DropColumn(
                name: "CountMissTaiko",
                table: "LeaderboardRx");

            migrationBuilder.RenameColumn(
                name: "PeppyPoints",
                table: "Scores",
                newName: "PerformancePoints");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PerformancePoints",
                table: "Scores",
                newName: "PeppyPoints");

            migrationBuilder.AddColumn<int>(
                name: "Count100Ctb",
                table: "LeaderboardStd",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Count100Mania",
                table: "LeaderboardStd",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Count100Osu",
                table: "LeaderboardStd",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Count100Taiko",
                table: "LeaderboardStd",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Count300Ctb",
                table: "LeaderboardStd",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Count300Mania",
                table: "LeaderboardStd",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Count300Osu",
                table: "LeaderboardStd",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Count300Taiko",
                table: "LeaderboardStd",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Count50Ctb",
                table: "LeaderboardStd",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Count50Mania",
                table: "LeaderboardStd",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Count50Osu",
                table: "LeaderboardStd",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Count50Taiko",
                table: "LeaderboardStd",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CountMissCtb",
                table: "LeaderboardStd",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CountMissMania",
                table: "LeaderboardStd",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CountMissOsu",
                table: "LeaderboardStd",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CountMissTaiko",
                table: "LeaderboardStd",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Count100Ctb",
                table: "LeaderboardRx",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Count100Osu",
                table: "LeaderboardRx",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Count100Taiko",
                table: "LeaderboardRx",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Count300Ctb",
                table: "LeaderboardRx",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Count300Osu",
                table: "LeaderboardRx",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Count300Taiko",
                table: "LeaderboardRx",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Count50Ctb",
                table: "LeaderboardRx",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Count50Osu",
                table: "LeaderboardRx",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Count50Taiko",
                table: "LeaderboardRx",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CountMissCtb",
                table: "LeaderboardRx",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CountMissOsu",
                table: "LeaderboardRx",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CountMissTaiko",
                table: "LeaderboardRx",
                nullable: false,
                defaultValue: 0);
        }
    }
}
