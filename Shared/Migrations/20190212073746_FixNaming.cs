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
