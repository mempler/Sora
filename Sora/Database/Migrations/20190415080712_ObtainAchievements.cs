using Microsoft.EntityFrameworkCore.Migrations;

namespace Sora.Database.Migrations
{
    public partial class ObtainAchievements : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "Achievements",
                table: "Users",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AlterColumn<ulong>(
                name: "BitId",
                table: "Achievements",
                nullable: false,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Achievements",
                table: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "BitId",
                table: "Achievements",
                nullable: false,
                oldClrType: typeof(ulong));
        }
    }
}
