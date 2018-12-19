using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Shared.Migrations
{
    public partial class Beatmaps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "BeatmapSets",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                              .Annotation("MySql:ValueGenerationStrategy",
                                          MySqlValueGenerationStrategy.IdentityColumn),
                    LastUpdate     = table.Column<DateTime>(nullable: false),
                    FavouriteCount = table.Column<int>(nullable: false),
                    PlayCount      = table.Column<int>(nullable: false),
                    PassCount      = table.Column<int>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_BeatmapSets", x => x.Id); });

            migrationBuilder.CreateTable(
                "Beatmaps",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                              .Annotation("MySql:ValueGenerationStrategy",
                                          MySqlValueGenerationStrategy.IdentityColumn),
                    RankedStatus     = table.Column<sbyte>(nullable: false),
                    RankedDate       = table.Column<DateTime>(nullable: false),
                    LastUpdate       = table.Column<DateTime>(nullable: false),
                    Artist           = table.Column<string>(nullable: false),
                    BeatmapSetId     = table.Column<int>(nullable: false),
                    Bpm              = table.Column<float>(nullable: false),
                    BeatmapCreator   = table.Column<string>(nullable: false),
                    BeatmapCreatorId = table.Column<int>(nullable: false),
                    Difficulty       = table.Column<double>(nullable: false),
                    Cs               = table.Column<float>(nullable: false),
                    Od               = table.Column<float>(nullable: false),
                    Ar               = table.Column<float>(nullable: false),
                    Hp               = table.Column<float>(nullable: false),
                    HitLength        = table.Column<int>(nullable: false),
                    Title            = table.Column<string>(nullable: false),
                    TotalLength      = table.Column<int>(nullable: false),
                    DifficultyName   = table.Column<string>(nullable: false),
                    FileMD5          = table.Column<string>(nullable: false),
                    PlayMode         = table.Column<byte>(nullable: false),
                    Tags             = table.Column<string>(nullable: false),
                    PlayCount        = table.Column<int>(nullable: false),
                    PassCount        = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beatmaps", x => x.Id);
                    table.ForeignKey(
                        "FK_Beatmaps_BeatmapSets_BeatmapSetId",
                        x => x.BeatmapSetId,
                        "BeatmapSets",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                "IX_Beatmaps_BeatmapSetId",
                "Beatmaps",
                "BeatmapSetId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "Beatmaps");

            migrationBuilder.DropTable(
                "BeatmapSets");
        }
    }
}