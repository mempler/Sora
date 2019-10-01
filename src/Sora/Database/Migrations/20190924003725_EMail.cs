using Microsoft.EntityFrameworkCore.Migrations;

namespace Sora.Database.Migrations
{
    public partial class EMail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EMail",
                table: "Users",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EMail",
                table: "Users");
        }
    }
}
