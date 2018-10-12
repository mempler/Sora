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
                "LeaderboardRx",
                table => new
                {
                    Id = table.Column<int>()
                              .Annotation("MySql:ValueGenerationStrategy",
                                          MySqlValueGenerationStrategy.IdentityColumn),
                    RankedScoreStd   = table.Column<ulong>(),
                    RankedScoreTaiko = table.Column<ulong>(),
                    RankedScoreCtb   = table.Column<ulong>(),
                    RankedScoreMania = table.Column<ulong>(),
                    TotalScoreStd    = table.Column<ulong>(),
                    TotalScoreTaiko  = table.Column<ulong>(),
                    TotalScoreCtb    = table.Column<ulong>(),
                    TotalScoreMania  = table.Column<ulong>(),
                    Count300Std      = table.Column<ulong>(),
                    Count300Taiko    = table.Column<ulong>(),
                    Count300Ctb      = table.Column<ulong>(),
                    Count300Mania    = table.Column<ulong>(),
                    Count100Std      = table.Column<ulong>(),
                    Count100Taiko    = table.Column<ulong>(),
                    Count100Ctb      = table.Column<ulong>(),
                    Count100Mania    = table.Column<ulong>(),
                    Count50Std       = table.Column<ulong>(),
                    Count50Taiko     = table.Column<ulong>(),
                    Count50Ctb       = table.Column<ulong>(),
                    Count50Mania     = table.Column<ulong>(),
                    CountMissStd     = table.Column<ulong>(),
                    CountMissTaiko   = table.Column<ulong>(),
                    CountMissCtb     = table.Column<ulong>(),
                    CountMissMania   = table.Column<ulong>(),
                    PlayCountStd     = table.Column<ulong>(),
                    PlayCountTaiko   = table.Column<ulong>(),
                    PlayCountCtb     = table.Column<ulong>(),
                    PlayCountMania   = table.Column<ulong>(),
                    PeppyPointsStd   = table.Column<double>(),
                    PeppyPointsTaiko = table.Column<double>(),
                    PeppyPointsCtb   = table.Column<double>(),
                    PeppyPointsMania = table.Column<double>()
                },
                constraints: table => { table.PrimaryKey("PK_LeaderboardRx", x => x.Id); });

            migrationBuilder.CreateTable(
                "LeaderboardStd",
                table => new
                {
                    Id = table.Column<int>()
                              .Annotation("MySql:ValueGenerationStrategy",
                                          MySqlValueGenerationStrategy.IdentityColumn),
                    RankedScoreStd   = table.Column<ulong>(),
                    RankedScoreTaiko = table.Column<ulong>(),
                    RankedScoreCtb   = table.Column<ulong>(),
                    RankedScoreMania = table.Column<ulong>(),
                    TotalScoreStd    = table.Column<ulong>(),
                    TotalScoreTaiko  = table.Column<ulong>(),
                    TotalScoreCtb    = table.Column<ulong>(),
                    TotalScoreMania  = table.Column<ulong>(),
                    Count300Std      = table.Column<ulong>(),
                    Count300Taiko    = table.Column<ulong>(),
                    Count300Ctb      = table.Column<ulong>(),
                    Count300Mania    = table.Column<ulong>(),
                    Count100Std      = table.Column<ulong>(),
                    Count100Taiko    = table.Column<ulong>(),
                    Count100Ctb      = table.Column<ulong>(),
                    Count100Mania    = table.Column<ulong>(),
                    Count50Std       = table.Column<ulong>(),
                    Count50Taiko     = table.Column<ulong>(),
                    Count50Ctb       = table.Column<ulong>(),
                    Count50Mania     = table.Column<ulong>(),
                    CountMissStd     = table.Column<ulong>(),
                    CountMissTaiko   = table.Column<ulong>(),
                    CountMissCtb     = table.Column<ulong>(),
                    CountMissMania   = table.Column<ulong>(),
                    PlayCountStd     = table.Column<ulong>(),
                    PlayCountTaiko   = table.Column<ulong>(),
                    PlayCountCtb     = table.Column<ulong>(),
                    PlayCountMania   = table.Column<ulong>(),
                    PeppyPointsStd   = table.Column<double>(),
                    PeppyPointsTaiko = table.Column<double>(),
                    PeppyPointsCtb   = table.Column<double>(),
                    PeppyPointsMania = table.Column<double>()
                },
                constraints: table => { table.PrimaryKey("PK_LeaderboardStd", x => x.Id); });

            migrationBuilder.CreateTable(
                "LeaderboardTouch",
                table => new
                {
                    Id = table.Column<int>()
                              .Annotation("MySql:ValueGenerationStrategy",
                                          MySqlValueGenerationStrategy.IdentityColumn),
                    RankedScoreStd   = table.Column<ulong>(),
                    RankedScoreTaiko = table.Column<ulong>(),
                    RankedScoreCtb   = table.Column<ulong>(),
                    RankedScoreMania = table.Column<ulong>(),
                    TotalScoreStd    = table.Column<ulong>(),
                    TotalScoreTaiko  = table.Column<ulong>(),
                    TotalScoreCtb    = table.Column<ulong>(),
                    TotalScoreMania  = table.Column<ulong>(),
                    Count300Std      = table.Column<ulong>(),
                    Count300Taiko    = table.Column<ulong>(),
                    Count300Ctb      = table.Column<ulong>(),
                    Count300Mania    = table.Column<ulong>(),
                    Count100Std      = table.Column<ulong>(),
                    Count100Taiko    = table.Column<ulong>(),
                    Count100Ctb      = table.Column<ulong>(),
                    Count100Mania    = table.Column<ulong>(),
                    Count50Std       = table.Column<ulong>(),
                    Count50Taiko     = table.Column<ulong>(),
                    Count50Ctb       = table.Column<ulong>(),
                    Count50Mania     = table.Column<ulong>(),
                    CountMissStd     = table.Column<ulong>(),
                    CountMissTaiko   = table.Column<ulong>(),
                    CountMissCtb     = table.Column<ulong>(),
                    CountMissMania   = table.Column<ulong>(),
                    PlayCountStd     = table.Column<ulong>(),
                    PlayCountTaiko   = table.Column<ulong>(),
                    PlayCountCtb     = table.Column<ulong>(),
                    PlayCountMania   = table.Column<ulong>(),
                    PeppyPointsStd   = table.Column<double>(),
                    PeppyPointsTaiko = table.Column<double>(),
                    PeppyPointsCtb   = table.Column<double>(),
                    PeppyPointsMania = table.Column<double>()
                },
                constraints: table => { table.PrimaryKey("PK_LeaderboardTouch", x => x.Id); });

            migrationBuilder.CreateTable(
                "Users",
                table => new
                {
                    Id = table.Column<int>()
                              .Annotation("MySql:ValueGenerationStrategy",
                                          MySqlValueGenerationStrategy.IdentityColumn),
                    Username   = table.Column<string>(),
                    Password   = table.Column<string>(),
                    Email      = table.Column<string>(),
                    Privileges = table.Column<int>()
                },
                constraints: table => { table.PrimaryKey("PK_Users", x => x.Id); });

            migrationBuilder.CreateTable(
                "UserStats",
                table => new
                {
                    Id = table.Column<int>()
                              .Annotation("MySql:ValueGenerationStrategy",
                                          MySqlValueGenerationStrategy.IdentityColumn),
                    CountryId = table.Column<byte>()
                },
                constraints: table => { table.PrimaryKey("PK_UserStats", x => x.Id); });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "LeaderboardRx");

            migrationBuilder.DropTable(
                "LeaderboardStd");

            migrationBuilder.DropTable(
                "LeaderboardTouch");

            migrationBuilder.DropTable(
                "Users");

            migrationBuilder.DropTable(
                "UserStats");
        }
    }
}