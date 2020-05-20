using Microsoft.EntityFrameworkCore.Migrations;

namespace Sora.Database.Migrations
{
    public partial class OptimizeDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "Users",
                type: "varchar(32)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "EMail",
                table: "Users",
                type: "varchar(64)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ReplayMd5",
                table: "Scores",
                type: "varchar(32)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FileMd5",
                table: "Scores",
                type: "varchar(32)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Beatmaps",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "Beatmaps",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "FileMd5",
                table: "Beatmaps",
                type: "varchar(32)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "DiffName",
                table: "Beatmaps",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Artist",
                table: "Beatmaps",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4");
            
            migrationBuilder.CreateIndex("IX_Beatmaps_SetId", "Beatmaps", "SetId", unique: false);
            migrationBuilder.CreateIndex("IX_Beatmaps_FileMd5", "Beatmaps", "FileMd5", unique: true);
            migrationBuilder.CreateIndex("IX_Beatmaps_RankedStatus", "Beatmaps", "RankedStatus", unique: false);

            migrationBuilder.CreateIndex("IX_Scores_FileMd5", "Scores", "FileMd5", unique: false);
            migrationBuilder.CreateIndex("IX_Scores_Mods", "Scores", "Mods", unique: false);
            migrationBuilder.CreateIndex("IX_Scores_PlayMode", "Scores", "PlayMode", unique: false);
            migrationBuilder.CreateIndex("IX_Scores_ReplayMd5", "Scores", "ReplayMd5", unique: true);

            migrationBuilder.CreateIndex("IX_Users_UserName", "Users", "UserName", unique: true);
            migrationBuilder.CreateIndex("IX_Users_EMail", "Users", "EMail", unique: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "Users",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(32)");

            migrationBuilder.AlterColumn<string>(
                name: "EMail",
                table: "Users",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(64)");

            migrationBuilder.AlterColumn<string>(
                name: "ReplayMd5",
                table: "Scores",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Beatmaps",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)");

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "Beatmaps",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)");

            migrationBuilder.AlterColumn<string>(
                name: "FileMd5",
                table: "Beatmaps",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(32)");

            migrationBuilder.AlterColumn<string>(
                name: "DiffName",
                table: "Beatmaps",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)");

            migrationBuilder.AlterColumn<string>(
                name: "Artist",
                table: "Beatmaps",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)");

            migrationBuilder.DropIndex("IX_Beatmaps_SetId");
            migrationBuilder.DropIndex("IX_Beatmaps_FileMd5");
            migrationBuilder.DropIndex("IX_Beatmaps_RankedStatus");

            migrationBuilder.DropIndex("IX_Scores_FileMd5");
            migrationBuilder.DropIndex("IX_Scores_Mods");
            migrationBuilder.DropIndex("IX_Scores_PlayMode");
            migrationBuilder.DropIndex("IX_Scores_ReplayMd5");

            migrationBuilder.DropIndex("IX_Users_UserName");
            migrationBuilder.DropIndex("IX_Users_EMail");
        }
    }
}
