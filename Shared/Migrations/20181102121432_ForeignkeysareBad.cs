using Microsoft.EntityFrameworkCore.Migrations;

namespace Shared.Migrations
{
    public partial class ForeignkeysareBad : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Scores_Users_UserId",
                table: "Scores");

            migrationBuilder.DropIndex(
                name: "IX_Scores_UserId",
                table: "Scores");

            migrationBuilder.AlterColumn<ulong>(
                name: "TotalScore",
                table: "Scores",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<ulong>(
                name: "CountMiss",
                table: "Scores",
                nullable: false,
                oldClrType: typeof(short));

            migrationBuilder.AlterColumn<ulong>(
                name: "CountKatu",
                table: "Scores",
                nullable: false,
                oldClrType: typeof(short));

            migrationBuilder.AlterColumn<ulong>(
                name: "CountGeki",
                table: "Scores",
                nullable: false,
                oldClrType: typeof(short));

            migrationBuilder.AlterColumn<ulong>(
                name: "Count50",
                table: "Scores",
                nullable: false,
                oldClrType: typeof(short));

            migrationBuilder.AlterColumn<ulong>(
                name: "Count300",
                table: "Scores",
                nullable: false,
                oldClrType: typeof(short));

            migrationBuilder.AlterColumn<ulong>(
                name: "Count100",
                table: "Scores",
                nullable: false,
                oldClrType: typeof(short));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "TotalScore",
                table: "Scores",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<short>(
                name: "CountMiss",
                table: "Scores",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<short>(
                name: "CountKatu",
                table: "Scores",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<short>(
                name: "CountGeki",
                table: "Scores",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<short>(
                name: "Count50",
                table: "Scores",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<short>(
                name: "Count300",
                table: "Scores",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<short>(
                name: "Count100",
                table: "Scores",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.CreateIndex(
                name: "IX_Scores_UserId",
                table: "Scores",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Scores_Users_UserId",
                table: "Scores",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
