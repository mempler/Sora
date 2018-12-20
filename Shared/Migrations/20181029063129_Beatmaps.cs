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
                    Id = table.Column<int>()
                              .Annotation("MySql:ValueGenerationStrategy",
                                          MySqlValueGenerationStrategy.IdentityColumn),
                    LastUpdate     = table.Column<DateTime>(),
                    FavouriteCount = table.Column<int>(),
                    PlayCount      = table.Column<int>(),
                    PassCount      = table.Column<int>()
                },
                constraints: table => { table.PrimaryKey("PK_BeatmapSets", x => x.Id); });

            migrationBuilder.CreateTable(
                "Beatmaps",
                table => new
                {
                    Id = table.Column<int>()
                              .Annotation("MySql:ValueGenerationStrategy",
                                          MySqlValueGenerationStrategy.IdentityColumn),
                    RankedStatus     = table.Column<sbyte>(),
                    RankedDate       = table.Column<DateTime>(),
                    LastUpdate       = table.Column<DateTime>(),
                    Artist           = table.Column<string>(),
                    BeatmapSetId     = table.Column<int>(),
                    Bpm              = table.Column<float>(),
                    BeatmapCreator   = table.Column<string>(),
                    BeatmapCreatorId = table.Column<int>(),
                    Difficulty       = table.Column<double>(),
                    Cs               = table.Column<float>(),
                    Od               = table.Column<float>(),
                    Ar               = table.Column<float>(),
                    Hp               = table.Column<float>(),
                    HitLength        = table.Column<int>(),
                    Title            = table.Column<string>(),
                    TotalLength      = table.Column<int>(),
                    DifficultyName   = table.Column<string>(),
                    FileMD5          = table.Column<string>(),
                    PlayMode         = table.Column<byte>(),
                    Tags             = table.Column<string>(),
                    PlayCount        = table.Column<int>(),
                    PassCount        = table.Column<int>()
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