using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JwtMusicNight.WebApi.Migrations
{
    public partial class mig_songfavourite : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SongFavorites",
                columns: table => new
                {
                    SongFavoriteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FavoriteDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SongId = table.Column<int>(type: "int", nullable: false),
                    AppUserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SongFavorites", x => x.SongFavoriteId);
                    table.ForeignKey(
                        name: "FK_SongFavorites_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SongFavorites_Songs_SongId",
                        column: x => x.SongId,
                        principalTable: "Songs",
                        principalColumn: "SongId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SongFavorites_AppUserId_SongId",
                table: "SongFavorites",
                columns: new[] { "AppUserId", "SongId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SongFavorites_SongId",
                table: "SongFavorites",
                column: "SongId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SongFavorites");
        }
    }
}
