using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sora.Database.Migrations
{
    public partial class StatusFlag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Users",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "StatusReason",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StatusUntil",
                table: "Users",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "StatusReason",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "StatusUntil",
                table: "Users");
        }
    }
}
