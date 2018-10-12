using Microsoft.EntityFrameworkCore.Migrations;

namespace Shared.Migrations
{
    public partial class NamingIssuese : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                "TotalScoreStd",
                "LeaderboardTouch",
                "TotalScoreOsu");

            migrationBuilder.RenameColumn(
                "RankedScoreStd",
                "LeaderboardTouch",
                "RankedScoreOsu");

            migrationBuilder.RenameColumn(
                "PlayCountStd",
                "LeaderboardTouch",
                "PlayCountOsu");

            migrationBuilder.RenameColumn(
                "PeppyPointsStd",
                "LeaderboardTouch",
                "PeppyPointsOsu");

            migrationBuilder.RenameColumn(
                "CountMissStd",
                "LeaderboardTouch",
                "CountMissOsu");

            migrationBuilder.RenameColumn(
                "Count50Std",
                "LeaderboardTouch",
                "Count50Osu");

            migrationBuilder.RenameColumn(
                "Count300Std",
                "LeaderboardTouch",
                "Count300Osu");

            migrationBuilder.RenameColumn(
                "Count100Std",
                "LeaderboardTouch",
                "Count100Osu");

            migrationBuilder.RenameColumn(
                "TotalScoreStd",
                "LeaderboardStd",
                "TotalScoreOsu");

            migrationBuilder.RenameColumn(
                "RankedScoreStd",
                "LeaderboardStd",
                "RankedScoreOsu");

            migrationBuilder.RenameColumn(
                "PlayCountStd",
                "LeaderboardStd",
                "PlayCountOsu");

            migrationBuilder.RenameColumn(
                "PeppyPointsStd",
                "LeaderboardStd",
                "PeppyPointsOsu");

            migrationBuilder.RenameColumn(
                "CountMissStd",
                "LeaderboardStd",
                "CountMissOsu");

            migrationBuilder.RenameColumn(
                "Count50Std",
                "LeaderboardStd",
                "Count50Osu");

            migrationBuilder.RenameColumn(
                "Count300Std",
                "LeaderboardStd",
                "Count300Osu");

            migrationBuilder.RenameColumn(
                "Count100Std",
                "LeaderboardStd",
                "Count100Osu");

            migrationBuilder.RenameColumn(
                "TotalScoreStd",
                "LeaderboardRx",
                "TotalScoreOsu");

            migrationBuilder.RenameColumn(
                "RankedScoreStd",
                "LeaderboardRx",
                "RankedScoreOsu");

            migrationBuilder.RenameColumn(
                "PlayCountStd",
                "LeaderboardRx",
                "PlayCountOsu");

            migrationBuilder.RenameColumn(
                "PeppyPointsStd",
                "LeaderboardRx",
                "PeppyPointsOsu");

            migrationBuilder.RenameColumn(
                "CountMissStd",
                "LeaderboardRx",
                "CountMissOsu");

            migrationBuilder.RenameColumn(
                "Count50Std",
                "LeaderboardRx",
                "Count50Osu");

            migrationBuilder.RenameColumn(
                "Count300Std",
                "LeaderboardRx",
                "Count300Osu");

            migrationBuilder.RenameColumn(
                "Count100Std",
                "LeaderboardRx",
                "Count100Osu");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                "TotalScoreOsu",
                "LeaderboardTouch",
                "TotalScoreStd");

            migrationBuilder.RenameColumn(
                "RankedScoreOsu",
                "LeaderboardTouch",
                "RankedScoreStd");

            migrationBuilder.RenameColumn(
                "PlayCountOsu",
                "LeaderboardTouch",
                "PlayCountStd");

            migrationBuilder.RenameColumn(
                "PeppyPointsOsu",
                "LeaderboardTouch",
                "PeppyPointsStd");

            migrationBuilder.RenameColumn(
                "CountMissOsu",
                "LeaderboardTouch",
                "CountMissStd");

            migrationBuilder.RenameColumn(
                "Count50Osu",
                "LeaderboardTouch",
                "Count50Std");

            migrationBuilder.RenameColumn(
                "Count300Osu",
                "LeaderboardTouch",
                "Count300Std");

            migrationBuilder.RenameColumn(
                "Count100Osu",
                "LeaderboardTouch",
                "Count100Std");

            migrationBuilder.RenameColumn(
                "TotalScoreOsu",
                "LeaderboardStd",
                "TotalScoreStd");

            migrationBuilder.RenameColumn(
                "RankedScoreOsu",
                "LeaderboardStd",
                "RankedScoreStd");

            migrationBuilder.RenameColumn(
                "PlayCountOsu",
                "LeaderboardStd",
                "PlayCountStd");

            migrationBuilder.RenameColumn(
                "PeppyPointsOsu",
                "LeaderboardStd",
                "PeppyPointsStd");

            migrationBuilder.RenameColumn(
                "CountMissOsu",
                "LeaderboardStd",
                "CountMissStd");

            migrationBuilder.RenameColumn(
                "Count50Osu",
                "LeaderboardStd",
                "Count50Std");

            migrationBuilder.RenameColumn(
                "Count300Osu",
                "LeaderboardStd",
                "Count300Std");

            migrationBuilder.RenameColumn(
                "Count100Osu",
                "LeaderboardStd",
                "Count100Std");

            migrationBuilder.RenameColumn(
                "TotalScoreOsu",
                "LeaderboardRx",
                "TotalScoreStd");

            migrationBuilder.RenameColumn(
                "RankedScoreOsu",
                "LeaderboardRx",
                "RankedScoreStd");

            migrationBuilder.RenameColumn(
                "PlayCountOsu",
                "LeaderboardRx",
                "PlayCountStd");

            migrationBuilder.RenameColumn(
                "PeppyPointsOsu",
                "LeaderboardRx",
                "PeppyPointsStd");

            migrationBuilder.RenameColumn(
                "CountMissOsu",
                "LeaderboardRx",
                "CountMissStd");

            migrationBuilder.RenameColumn(
                "Count50Osu",
                "LeaderboardRx",
                "Count50Std");

            migrationBuilder.RenameColumn(
                "Count300Osu",
                "LeaderboardRx",
                "Count300Std");

            migrationBuilder.RenameColumn(
                "Count100Osu",
                "LeaderboardRx",
                "Count100Std");
        }
    }
}