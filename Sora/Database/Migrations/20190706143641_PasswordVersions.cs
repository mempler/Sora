using Microsoft.EntityFrameworkCore.Migrations;

namespace Sora.Database.Migrations
{
    public partial class PasswordVersions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PasswordVersion",
                table: "Users",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "TotalScore",
                table: "Scores",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<int>(
                name: "CountMiss",
                table: "Scores",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<int>(
                name: "CountKatu",
                table: "Scores",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<int>(
                name: "CountGeki",
                table: "Scores",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<int>(
                name: "Count50",
                table: "Scores",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<int>(
                name: "Count300",
                table: "Scores",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<int>(
                name: "Count100",
                table: "Scores",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<int>(
                name: "CountMissTaiko",
                table: "LeaderboardStd",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<int>(
                name: "CountMissOsu",
                table: "LeaderboardStd",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<int>(
                name: "CountMissMania",
                table: "LeaderboardStd",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<int>(
                name: "CountMissCtb",
                table: "LeaderboardStd",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<int>(
                name: "Count50Taiko",
                table: "LeaderboardStd",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<int>(
                name: "Count50Osu",
                table: "LeaderboardStd",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<int>(
                name: "Count50Mania",
                table: "LeaderboardStd",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<int>(
                name: "Count50Ctb",
                table: "LeaderboardStd",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<int>(
                name: "Count300Taiko",
                table: "LeaderboardStd",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<int>(
                name: "Count300Osu",
                table: "LeaderboardStd",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<int>(
                name: "Count300Mania",
                table: "LeaderboardStd",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<int>(
                name: "Count300Ctb",
                table: "LeaderboardStd",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<int>(
                name: "Count100Taiko",
                table: "LeaderboardStd",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<int>(
                name: "Count100Osu",
                table: "LeaderboardStd",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<int>(
                name: "Count100Mania",
                table: "LeaderboardStd",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<int>(
                name: "Count100Ctb",
                table: "LeaderboardStd",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<int>(
                name: "CountMissTaiko",
                table: "LeaderboardRx",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<int>(
                name: "CountMissOsu",
                table: "LeaderboardRx",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<int>(
                name: "CountMissCtb",
                table: "LeaderboardRx",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<int>(
                name: "Count50Taiko",
                table: "LeaderboardRx",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<int>(
                name: "Count50Osu",
                table: "LeaderboardRx",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<int>(
                name: "Count50Ctb",
                table: "LeaderboardRx",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<int>(
                name: "Count300Taiko",
                table: "LeaderboardRx",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<int>(
                name: "Count300Osu",
                table: "LeaderboardRx",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<int>(
                name: "Count300Ctb",
                table: "LeaderboardRx",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<int>(
                name: "Count100Taiko",
                table: "LeaderboardRx",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<int>(
                name: "Count100Osu",
                table: "LeaderboardRx",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<int>(
                name: "Count100Ctb",
                table: "LeaderboardRx",
                nullable: false,
                oldClrType: typeof(ulong));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordVersion",
                table: "Users");

            migrationBuilder.AlterColumn<ulong>(
                name: "TotalScore",
                table: "Scores",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<ulong>(
                name: "CountMiss",
                table: "Scores",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<ulong>(
                name: "CountKatu",
                table: "Scores",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<ulong>(
                name: "CountGeki",
                table: "Scores",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<ulong>(
                name: "Count50",
                table: "Scores",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<ulong>(
                name: "Count300",
                table: "Scores",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<ulong>(
                name: "Count100",
                table: "Scores",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<ulong>(
                name: "CountMissTaiko",
                table: "LeaderboardStd",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<ulong>(
                name: "CountMissOsu",
                table: "LeaderboardStd",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<ulong>(
                name: "CountMissMania",
                table: "LeaderboardStd",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<ulong>(
                name: "CountMissCtb",
                table: "LeaderboardStd",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<ulong>(
                name: "Count50Taiko",
                table: "LeaderboardStd",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<ulong>(
                name: "Count50Osu",
                table: "LeaderboardStd",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<ulong>(
                name: "Count50Mania",
                table: "LeaderboardStd",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<ulong>(
                name: "Count50Ctb",
                table: "LeaderboardStd",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<ulong>(
                name: "Count300Taiko",
                table: "LeaderboardStd",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<ulong>(
                name: "Count300Osu",
                table: "LeaderboardStd",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<ulong>(
                name: "Count300Mania",
                table: "LeaderboardStd",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<ulong>(
                name: "Count300Ctb",
                table: "LeaderboardStd",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<ulong>(
                name: "Count100Taiko",
                table: "LeaderboardStd",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<ulong>(
                name: "Count100Osu",
                table: "LeaderboardStd",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<ulong>(
                name: "Count100Mania",
                table: "LeaderboardStd",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<ulong>(
                name: "Count100Ctb",
                table: "LeaderboardStd",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<ulong>(
                name: "CountMissTaiko",
                table: "LeaderboardRx",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<ulong>(
                name: "CountMissOsu",
                table: "LeaderboardRx",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<ulong>(
                name: "CountMissCtb",
                table: "LeaderboardRx",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<ulong>(
                name: "Count50Taiko",
                table: "LeaderboardRx",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<ulong>(
                name: "Count50Osu",
                table: "LeaderboardRx",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<ulong>(
                name: "Count50Ctb",
                table: "LeaderboardRx",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<ulong>(
                name: "Count300Taiko",
                table: "LeaderboardRx",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<ulong>(
                name: "Count300Osu",
                table: "LeaderboardRx",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<ulong>(
                name: "Count300Ctb",
                table: "LeaderboardRx",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<ulong>(
                name: "Count100Taiko",
                table: "LeaderboardRx",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<ulong>(
                name: "Count100Osu",
                table: "LeaderboardRx",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<ulong>(
                name: "Count100Ctb",
                table: "LeaderboardRx",
                nullable: false,
                oldClrType: typeof(int));
        }
    }
}
