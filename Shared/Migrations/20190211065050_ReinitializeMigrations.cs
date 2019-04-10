#region LICENSE
/*
    Sora - A Modular Bancho written in C#
    Copyright (C) 2019 Robin A. P.

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as
    published by the Free Software Foundation, either version 3 of the
    License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
#endregion

using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

// ReSharper disable All

namespace Shared.Migrations
{
    public partial class ReinitializeMigrations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"ALTER DATABASE CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;");
            
            migrationBuilder.CreateTable(
                name: "Beatmaps",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RankedStatus = table.Column<sbyte>(nullable: false),
                    RankedDate = table.Column<DateTime>(nullable: false),
                    LastUpdate = table.Column<DateTime>(nullable: false),
                    Artist = table.Column<string>(nullable: false),
                    BeatmapSetId = table.Column<int>(nullable: false),
                    Bpm = table.Column<float>(nullable: false),
                    BeatmapCreator = table.Column<string>(nullable: false),
                    BeatmapCreatorId = table.Column<int>(nullable: false),
                    Difficulty = table.Column<double>(nullable: false),
                    Cs = table.Column<float>(nullable: false),
                    Od = table.Column<float>(nullable: false),
                    Ar = table.Column<float>(nullable: false),
                    Hp = table.Column<float>(nullable: false),
                    HitLength = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    TotalLength = table.Column<int>(nullable: false),
                    DifficultyName = table.Column<string>(nullable: false),
                    FileMd5 = table.Column<string>(nullable: false),
                    PlayMode = table.Column<byte>(nullable: false),
                    Tags = table.Column<string>(nullable: false),
                    PlayCount = table.Column<int>(nullable: false),
                    PassCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beatmaps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BeatmapSets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LastUpdate = table.Column<DateTime>(nullable: false),
                    FavouriteCount = table.Column<int>(nullable: false),
                    PlayCount = table.Column<int>(nullable: false),
                    PassCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeatmapSets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Friends",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: false),
                    FriendId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Friends", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LeaderboardRx",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RankedScoreOsu = table.Column<ulong>(nullable: false),
                    RankedScoreTaiko = table.Column<ulong>(nullable: false),
                    RankedScoreCtb = table.Column<ulong>(nullable: false),
                    RankedScoreMania = table.Column<ulong>(nullable: false),
                    TotalScoreOsu = table.Column<ulong>(nullable: false),
                    TotalScoreTaiko = table.Column<ulong>(nullable: false),
                    TotalScoreCtb = table.Column<ulong>(nullable: false),
                    TotalScoreMania = table.Column<ulong>(nullable: false),
                    Count300Osu = table.Column<ulong>(nullable: false),
                    Count300Taiko = table.Column<ulong>(nullable: false),
                    Count300Ctb = table.Column<ulong>(nullable: false),
                    Count300Mania = table.Column<ulong>(nullable: false),
                    Count100Osu = table.Column<ulong>(nullable: false),
                    Count100Taiko = table.Column<ulong>(nullable: false),
                    Count100Ctb = table.Column<ulong>(nullable: false),
                    Count100Mania = table.Column<ulong>(nullable: false),
                    Count50Osu = table.Column<ulong>(nullable: false),
                    Count50Taiko = table.Column<ulong>(nullable: false),
                    Count50Ctb = table.Column<ulong>(nullable: false),
                    Count50Mania = table.Column<ulong>(nullable: false),
                    CountMissOsu = table.Column<ulong>(nullable: false),
                    CountMissTaiko = table.Column<ulong>(nullable: false),
                    CountMissCtb = table.Column<ulong>(nullable: false),
                    CountMissMania = table.Column<ulong>(nullable: false),
                    PlayCountOsu = table.Column<ulong>(nullable: false),
                    PlayCountTaiko = table.Column<ulong>(nullable: false),
                    PlayCountCtb = table.Column<ulong>(nullable: false),
                    PlayCountMania = table.Column<ulong>(nullable: false),
                    PerformancePointsOsu = table.Column<double>(nullable: false),
                    PerformancePointsTaiko = table.Column<double>(nullable: false),
                    PerformancePointsCtb = table.Column<double>(nullable: false),
                    PerformancePerformance = table.Column<double>(nullable: false)
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
                    RankedScoreOsu = table.Column<ulong>(nullable: false),
                    RankedScoreTaiko = table.Column<ulong>(nullable: false),
                    RankedScoreCtb = table.Column<ulong>(nullable: false),
                    RankedScoreMania = table.Column<ulong>(nullable: false),
                    TotalScoreOsu = table.Column<ulong>(nullable: false),
                    TotalScoreTaiko = table.Column<ulong>(nullable: false),
                    TotalScoreCtb = table.Column<ulong>(nullable: false),
                    TotalScoreMania = table.Column<ulong>(nullable: false),
                    Count300Osu = table.Column<ulong>(nullable: false),
                    Count300Taiko = table.Column<ulong>(nullable: false),
                    Count300Ctb = table.Column<ulong>(nullable: false),
                    Count300Mania = table.Column<ulong>(nullable: false),
                    Count100Osu = table.Column<ulong>(nullable: false),
                    Count100Taiko = table.Column<ulong>(nullable: false),
                    Count100Ctb = table.Column<ulong>(nullable: false),
                    Count100Mania = table.Column<ulong>(nullable: false),
                    Count50Osu = table.Column<ulong>(nullable: false),
                    Count50Taiko = table.Column<ulong>(nullable: false),
                    Count50Ctb = table.Column<ulong>(nullable: false),
                    Count50Mania = table.Column<ulong>(nullable: false),
                    CountMissOsu = table.Column<ulong>(nullable: false),
                    CountMissTaiko = table.Column<ulong>(nullable: false),
                    CountMissCtb = table.Column<ulong>(nullable: false),
                    CountMissMania = table.Column<ulong>(nullable: false),
                    PlayCountOsu = table.Column<ulong>(nullable: false),
                    PlayCountTaiko = table.Column<ulong>(nullable: false),
                    PlayCountCtb = table.Column<ulong>(nullable: false),
                    PlayCountMania = table.Column<ulong>(nullable: false),
                    PerformancePointsOsu = table.Column<double>(nullable: false),
                    PerformancePointsTaiko = table.Column<double>(nullable: false),
                    PerformancePointsCtb = table.Column<double>(nullable: false),
                    PerformancePointsMania = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaderboardStd", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Scores",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: false),
                    FileMd5 = table.Column<string>(nullable: false),
                    ScoreMd5 = table.Column<string>(nullable: false),
                    ReplayMd5 = table.Column<string>(nullable: false),
                    TotalScore = table.Column<ulong>(nullable: false),
                    MaxCombo = table.Column<short>(nullable: false),
                    PlayMode = table.Column<byte>(nullable: false),
                    Mods = table.Column<uint>(nullable: false),
                    Count300 = table.Column<ulong>(nullable: false),
                    Count100 = table.Column<ulong>(nullable: false),
                    Count50 = table.Column<ulong>(nullable: false),
                    CountMiss = table.Column<ulong>(nullable: false),
                    CountGeki = table.Column<ulong>(nullable: false),
                    CountKatu = table.Column<ulong>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Accuracy = table.Column<double>(nullable: false),
                    PeppyPoints = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scores", x => x.Id);
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
                name: "Beatmaps");

            migrationBuilder.DropTable(
                name: "BeatmapSets");

            migrationBuilder.DropTable(
                name: "Friends");

            migrationBuilder.DropTable(
                name: "LeaderboardRx");

            migrationBuilder.DropTable(
                name: "LeaderboardStd");

            migrationBuilder.DropTable(
                name: "Scores");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "UserStats");
        }
    }
}
