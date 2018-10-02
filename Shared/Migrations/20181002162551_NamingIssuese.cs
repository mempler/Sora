using Microsoft.EntityFrameworkCore.Migrations;

namespace Shared.Migrations
{
    public partial class NamingIssuese : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalScoreStd",
                table: "LeaderboardTouch",
                newName: "TotalScoreOsu");

            migrationBuilder.RenameColumn(
                name: "RankedScoreStd",
                table: "LeaderboardTouch",
                newName: "RankedScoreOsu");

            migrationBuilder.RenameColumn(
                name: "PlayCountStd",
                table: "LeaderboardTouch",
                newName: "PlayCountOsu");

            migrationBuilder.RenameColumn(
                name: "PeppyPointsStd",
                table: "LeaderboardTouch",
                newName: "PeppyPointsOsu");

            migrationBuilder.RenameColumn(
                name: "CountMissStd",
                table: "LeaderboardTouch",
                newName: "CountMissOsu");

            migrationBuilder.RenameColumn(
                name: "Count50Std",
                table: "LeaderboardTouch",
                newName: "Count50Osu");

            migrationBuilder.RenameColumn(
                name: "Count300Std",
                table: "LeaderboardTouch",
                newName: "Count300Osu");

            migrationBuilder.RenameColumn(
                name: "Count100Std",
                table: "LeaderboardTouch",
                newName: "Count100Osu");

            migrationBuilder.RenameColumn(
                name: "TotalScoreStd",
                table: "LeaderboardStd",
                newName: "TotalScoreOsu");

            migrationBuilder.RenameColumn(
                name: "RankedScoreStd",
                table: "LeaderboardStd",
                newName: "RankedScoreOsu");

            migrationBuilder.RenameColumn(
                name: "PlayCountStd",
                table: "LeaderboardStd",
                newName: "PlayCountOsu");

            migrationBuilder.RenameColumn(
                name: "PeppyPointsStd",
                table: "LeaderboardStd",
                newName: "PeppyPointsOsu");

            migrationBuilder.RenameColumn(
                name: "CountMissStd",
                table: "LeaderboardStd",
                newName: "CountMissOsu");

            migrationBuilder.RenameColumn(
                name: "Count50Std",
                table: "LeaderboardStd",
                newName: "Count50Osu");

            migrationBuilder.RenameColumn(
                name: "Count300Std",
                table: "LeaderboardStd",
                newName: "Count300Osu");

            migrationBuilder.RenameColumn(
                name: "Count100Std",
                table: "LeaderboardStd",
                newName: "Count100Osu");

            migrationBuilder.RenameColumn(
                name: "TotalScoreStd",
                table: "LeaderboardRx",
                newName: "TotalScoreOsu");

            migrationBuilder.RenameColumn(
                name: "RankedScoreStd",
                table: "LeaderboardRx",
                newName: "RankedScoreOsu");

            migrationBuilder.RenameColumn(
                name: "PlayCountStd",
                table: "LeaderboardRx",
                newName: "PlayCountOsu");

            migrationBuilder.RenameColumn(
                name: "PeppyPointsStd",
                table: "LeaderboardRx",
                newName: "PeppyPointsOsu");

            migrationBuilder.RenameColumn(
                name: "CountMissStd",
                table: "LeaderboardRx",
                newName: "CountMissOsu");

            migrationBuilder.RenameColumn(
                name: "Count50Std",
                table: "LeaderboardRx",
                newName: "Count50Osu");

            migrationBuilder.RenameColumn(
                name: "Count300Std",
                table: "LeaderboardRx",
                newName: "Count300Osu");

            migrationBuilder.RenameColumn(
                name: "Count100Std",
                table: "LeaderboardRx",
                newName: "Count100Osu");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalScoreOsu",
                table: "LeaderboardTouch",
                newName: "TotalScoreStd");

            migrationBuilder.RenameColumn(
                name: "RankedScoreOsu",
                table: "LeaderboardTouch",
                newName: "RankedScoreStd");

            migrationBuilder.RenameColumn(
                name: "PlayCountOsu",
                table: "LeaderboardTouch",
                newName: "PlayCountStd");

            migrationBuilder.RenameColumn(
                name: "PeppyPointsOsu",
                table: "LeaderboardTouch",
                newName: "PeppyPointsStd");

            migrationBuilder.RenameColumn(
                name: "CountMissOsu",
                table: "LeaderboardTouch",
                newName: "CountMissStd");

            migrationBuilder.RenameColumn(
                name: "Count50Osu",
                table: "LeaderboardTouch",
                newName: "Count50Std");

            migrationBuilder.RenameColumn(
                name: "Count300Osu",
                table: "LeaderboardTouch",
                newName: "Count300Std");

            migrationBuilder.RenameColumn(
                name: "Count100Osu",
                table: "LeaderboardTouch",
                newName: "Count100Std");

            migrationBuilder.RenameColumn(
                name: "TotalScoreOsu",
                table: "LeaderboardStd",
                newName: "TotalScoreStd");

            migrationBuilder.RenameColumn(
                name: "RankedScoreOsu",
                table: "LeaderboardStd",
                newName: "RankedScoreStd");

            migrationBuilder.RenameColumn(
                name: "PlayCountOsu",
                table: "LeaderboardStd",
                newName: "PlayCountStd");

            migrationBuilder.RenameColumn(
                name: "PeppyPointsOsu",
                table: "LeaderboardStd",
                newName: "PeppyPointsStd");

            migrationBuilder.RenameColumn(
                name: "CountMissOsu",
                table: "LeaderboardStd",
                newName: "CountMissStd");

            migrationBuilder.RenameColumn(
                name: "Count50Osu",
                table: "LeaderboardStd",
                newName: "Count50Std");

            migrationBuilder.RenameColumn(
                name: "Count300Osu",
                table: "LeaderboardStd",
                newName: "Count300Std");

            migrationBuilder.RenameColumn(
                name: "Count100Osu",
                table: "LeaderboardStd",
                newName: "Count100Std");

            migrationBuilder.RenameColumn(
                name: "TotalScoreOsu",
                table: "LeaderboardRx",
                newName: "TotalScoreStd");

            migrationBuilder.RenameColumn(
                name: "RankedScoreOsu",
                table: "LeaderboardRx",
                newName: "RankedScoreStd");

            migrationBuilder.RenameColumn(
                name: "PlayCountOsu",
                table: "LeaderboardRx",
                newName: "PlayCountStd");

            migrationBuilder.RenameColumn(
                name: "PeppyPointsOsu",
                table: "LeaderboardRx",
                newName: "PeppyPointsStd");

            migrationBuilder.RenameColumn(
                name: "CountMissOsu",
                table: "LeaderboardRx",
                newName: "CountMissStd");

            migrationBuilder.RenameColumn(
                name: "Count50Osu",
                table: "LeaderboardRx",
                newName: "Count50Std");

            migrationBuilder.RenameColumn(
                name: "Count300Osu",
                table: "LeaderboardRx",
                newName: "Count300Std");

            migrationBuilder.RenameColumn(
                name: "Count100Osu",
                table: "LeaderboardRx",
                newName: "Count100Std");
        }
    }
}
