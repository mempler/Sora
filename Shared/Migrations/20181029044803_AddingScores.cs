using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Shared.Migrations
{
    public partial class AddingScores : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "Scores",
                table => new
                {
                    Id = table.Column<int>()
                              .Annotation("MySql:ValueGenerationStrategy",
                                          MySqlValueGenerationStrategy.IdentityColumn),
                    UserId      = table.Column<int>(),
                    FileMD5     = table.Column<string>(),
                    ScoreMD5    = table.Column<string>(),
                    ReplayMD5   = table.Column<string>(),
                    TotalScore  = table.Column<int>(),
                    MaxCombo    = table.Column<short>(),
                    PlayMode    = table.Column<byte>(),
                    Mods        = table.Column<uint>(),
                    Count300    = table.Column<short>(),
                    Count100    = table.Column<short>(),
                    Count50     = table.Column<short>(),
                    CountMiss   = table.Column<short>(),
                    CountGeki   = table.Column<short>(),
                    CountKatu   = table.Column<short>(),
                    Date        = table.Column<DateTime>(),
                    Accuracy    = table.Column<double>(),
                    PeppyPoints = table.Column<double>()
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scores", x => x.Id);
                    table.ForeignKey(
                        "FK_Scores_Users_UserId",
                        x => x.UserId,
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                "IX_Scores_UserId",
                "Scores",
                "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("Scores");
        }
    }
}