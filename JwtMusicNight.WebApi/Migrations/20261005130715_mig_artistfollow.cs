using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JwtMusicNight.WebApi.Migrations
{
    public partial class mig_artistfollow : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArtistFollows",
                columns: table => new
                {
                    ArtistFollowId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FollowDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArtistId = table.Column<int>(type: "int", nullable: false),
                    AppUserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtistFollows", x => x.ArtistFollowId);
                    table.ForeignKey(
                        name: "FK_ArtistFollows_Artists_ArtistId",
                        column: x => x.ArtistId,
                        principalTable: "Artists",
                        principalColumn: "ArtistId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArtistFollows_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArtistFollows_AppUserId_ArtistId",
                table: "ArtistFollows",
                columns: new[] { "AppUserId", "ArtistId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ArtistFollows_ArtistId",
                table: "ArtistFollows",
                column: "ArtistId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArtistFollows");
        }
    }
}
