using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sora.Database.Migrations
{
    public partial class OAuth2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OAuthClients",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    OwnerId = table.Column<int>(nullable: false),
                    Secret = table.Column<string>(nullable: true),
                    Flags = table.Column<int>(nullable: false),
                    Disabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OAuthClients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OAuthClients_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OAuthClients_OwnerId",
                table: "OAuthClients",
                column: "OwnerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OAuthClients");
        }
    }
}
