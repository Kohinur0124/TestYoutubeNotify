using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestYoutube.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChannelYoutube",
                columns: table => new
                {
                    ChannelId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ChannelTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastCheckDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelYoutube", x => x.ChannelId);
                });

            migrationBuilder.CreateTable(
                name: "UserChannels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChatIdTelegram = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChannelYoutubeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StartDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserChannels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserChannels_ChannelYoutube_ChannelYoutubeId",
                        column: x => x.ChannelYoutubeId,
                        principalTable: "ChannelYoutube",
                        principalColumn: "ChannelId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserChannels_ChannelYoutubeId",
                table: "UserChannels",
                column: "ChannelYoutubeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserChannels");

            migrationBuilder.DropTable(
                name: "ChannelYoutube");
        }
    }
}
