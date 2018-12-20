using Microsoft.EntityFrameworkCore.Migrations;

namespace Shared.Migrations
{
    public partial class FixingNamingIssues : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                "ScoreMD5",
                "Scores",
                "ScoreMd5");

            migrationBuilder.RenameColumn(
                "ReplayMD5",
                "Scores",
                "ReplayMd5");

            migrationBuilder.RenameColumn(
                "FileMD5",
                "Scores",
                "FileMd5");

            migrationBuilder.RenameColumn(
                "FileMD5",
                "Beatmaps",
                "FileMd5");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                "ScoreMd5",
                "Scores",
                "ScoreMD5");

            migrationBuilder.RenameColumn(
                "ReplayMd5",
                "Scores",
                "ReplayMD5");

            migrationBuilder.RenameColumn(
                "FileMd5",
                "Scores",
                "FileMD5");

            migrationBuilder.RenameColumn(
                "FileMd5",
                "Beatmaps",
                "FileMD5");
        }
    }
}
