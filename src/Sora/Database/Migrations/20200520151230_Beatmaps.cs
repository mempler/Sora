using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sora.Database.Migrations
{
    public partial class Beatmaps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Beatmaps",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SetId = table.Column<int>(nullable: false),
                    RankedStatus = table.Column<int>(nullable: false),
                    FileMd5 = table.Column<string>(nullable: false),
                    PlayMode = table.Column<byte>(nullable: false),
                    Artist = table.Column<string>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    DiffName = table.Column<string>(nullable: false),
                    FileName = table.Column<string>(nullable: false),
                    Flags = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beatmaps", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Beatmaps");
        }
    }
}
