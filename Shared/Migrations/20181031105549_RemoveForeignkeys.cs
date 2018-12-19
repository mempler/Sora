using Microsoft.EntityFrameworkCore.Migrations;

namespace Shared.Migrations
{
    public partial class RemoveForeignkeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Beatmaps_BeatmapSets_BeatmapSetId",
                table: "Beatmaps");

            migrationBuilder.DropIndex(
                name: "IX_Beatmaps_BeatmapSetId",
                table: "Beatmaps");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Beatmaps_BeatmapSetId",
                table: "Beatmaps",
                column: "BeatmapSetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Beatmaps_BeatmapSets_BeatmapSetId",
                table: "Beatmaps",
                column: "BeatmapSetId",
                principalTable: "BeatmapSets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
