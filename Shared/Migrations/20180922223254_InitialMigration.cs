#region copyright
/*
MIT License

Copyright (c) 2018 Robin A. P.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
#endregion

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Shared.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LeaderboardRx",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RankedScoreStd = table.Column<ulong>(nullable: false),
                    RankedScoreTaiko = table.Column<ulong>(nullable: false),
                    RankedScoreCtb = table.Column<ulong>(nullable: false),
                    RankedScoreMania = table.Column<ulong>(nullable: false),
                    TotalScoreStd = table.Column<ulong>(nullable: false),
                    TotalScoreTaiko = table.Column<ulong>(nullable: false),
                    TotalScoreCtb = table.Column<ulong>(nullable: false),
                    TotalScoreMania = table.Column<ulong>(nullable: false),
                    Count300Std = table.Column<ulong>(nullable: false),
                    Count300Taiko = table.Column<ulong>(nullable: false),
                    Count300Ctb = table.Column<ulong>(nullable: false),
                    Count300Mania = table.Column<ulong>(nullable: false),
                    Count100Std = table.Column<ulong>(nullable: false),
                    Count100Taiko = table.Column<ulong>(nullable: false),
                    Count100Ctb = table.Column<ulong>(nullable: false),
                    Count100Mania = table.Column<ulong>(nullable: false),
                    Count50Std = table.Column<ulong>(nullable: false),
                    Count50Taiko = table.Column<ulong>(nullable: false),
                    Count50Ctb = table.Column<ulong>(nullable: false),
                    Count50Mania = table.Column<ulong>(nullable: false),
                    CountMissStd = table.Column<ulong>(nullable: false),
                    CountMissTaiko = table.Column<ulong>(nullable: false),
                    CountMissCtb = table.Column<ulong>(nullable: false),
                    CountMissMania = table.Column<ulong>(nullable: false),
                    PlayCountStd = table.Column<ulong>(nullable: false),
                    PlayCountTaiko = table.Column<ulong>(nullable: false),
                    PlayCountCtb = table.Column<ulong>(nullable: false),
                    PlayCountMania = table.Column<ulong>(nullable: false),
                    PeppyPointsStd = table.Column<double>(nullable: false),
                    PeppyPointsTaiko = table.Column<double>(nullable: false),
                    PeppyPointsCtb = table.Column<double>(nullable: false),
                    PeppyPointsMania = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaderboardRx", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LeaderboardStd",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RankedScoreStd = table.Column<ulong>(nullable: false),
                    RankedScoreTaiko = table.Column<ulong>(nullable: false),
                    RankedScoreCtb = table.Column<ulong>(nullable: false),
                    RankedScoreMania = table.Column<ulong>(nullable: false),
                    TotalScoreStd = table.Column<ulong>(nullable: false),
                    TotalScoreTaiko = table.Column<ulong>(nullable: false),
                    TotalScoreCtb = table.Column<ulong>(nullable: false),
                    TotalScoreMania = table.Column<ulong>(nullable: false),
                    Count300Std = table.Column<ulong>(nullable: false),
                    Count300Taiko = table.Column<ulong>(nullable: false),
                    Count300Ctb = table.Column<ulong>(nullable: false),
                    Count300Mania = table.Column<ulong>(nullable: false),
                    Count100Std = table.Column<ulong>(nullable: false),
                    Count100Taiko = table.Column<ulong>(nullable: false),
                    Count100Ctb = table.Column<ulong>(nullable: false),
                    Count100Mania = table.Column<ulong>(nullable: false),
                    Count50Std = table.Column<ulong>(nullable: false),
                    Count50Taiko = table.Column<ulong>(nullable: false),
                    Count50Ctb = table.Column<ulong>(nullable: false),
                    Count50Mania = table.Column<ulong>(nullable: false),
                    CountMissStd = table.Column<ulong>(nullable: false),
                    CountMissTaiko = table.Column<ulong>(nullable: false),
                    CountMissCtb = table.Column<ulong>(nullable: false),
                    CountMissMania = table.Column<ulong>(nullable: false),
                    PlayCountStd = table.Column<ulong>(nullable: false),
                    PlayCountTaiko = table.Column<ulong>(nullable: false),
                    PlayCountCtb = table.Column<ulong>(nullable: false),
                    PlayCountMania = table.Column<ulong>(nullable: false),
                    PeppyPointsStd = table.Column<double>(nullable: false),
                    PeppyPointsTaiko = table.Column<double>(nullable: false),
                    PeppyPointsCtb = table.Column<double>(nullable: false),
                    PeppyPointsMania = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaderboardStd", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LeaderboardTouch",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RankedScoreStd = table.Column<ulong>(nullable: false),
                    RankedScoreTaiko = table.Column<ulong>(nullable: false),
                    RankedScoreCtb = table.Column<ulong>(nullable: false),
                    RankedScoreMania = table.Column<ulong>(nullable: false),
                    TotalScoreStd = table.Column<ulong>(nullable: false),
                    TotalScoreTaiko = table.Column<ulong>(nullable: false),
                    TotalScoreCtb = table.Column<ulong>(nullable: false),
                    TotalScoreMania = table.Column<ulong>(nullable: false),
                    Count300Std = table.Column<ulong>(nullable: false),
                    Count300Taiko = table.Column<ulong>(nullable: false),
                    Count300Ctb = table.Column<ulong>(nullable: false),
                    Count300Mania = table.Column<ulong>(nullable: false),
                    Count100Std = table.Column<ulong>(nullable: false),
                    Count100Taiko = table.Column<ulong>(nullable: false),
                    Count100Ctb = table.Column<ulong>(nullable: false),
                    Count100Mania = table.Column<ulong>(nullable: false),
                    Count50Std = table.Column<ulong>(nullable: false),
                    Count50Taiko = table.Column<ulong>(nullable: false),
                    Count50Ctb = table.Column<ulong>(nullable: false),
                    Count50Mania = table.Column<ulong>(nullable: false),
                    CountMissStd = table.Column<ulong>(nullable: false),
                    CountMissTaiko = table.Column<ulong>(nullable: false),
                    CountMissCtb = table.Column<ulong>(nullable: false),
                    CountMissMania = table.Column<ulong>(nullable: false),
                    PlayCountStd = table.Column<ulong>(nullable: false),
                    PlayCountTaiko = table.Column<ulong>(nullable: false),
                    PlayCountCtb = table.Column<ulong>(nullable: false),
                    PlayCountMania = table.Column<ulong>(nullable: false),
                    PeppyPointsStd = table.Column<double>(nullable: false),
                    PeppyPointsTaiko = table.Column<double>(nullable: false),
                    PeppyPointsCtb = table.Column<double>(nullable: false),
                    PeppyPointsMania = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaderboardTouch", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Username = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    Privileges = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserStats",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CountryId = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserStats", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LeaderboardRx");

            migrationBuilder.DropTable(
                name: "LeaderboardStd");

            migrationBuilder.DropTable(
                name: "LeaderboardTouch");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "UserStats");
        }
    }
}
