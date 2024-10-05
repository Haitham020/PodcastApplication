using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PodcastApplication.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEpisodeIdinPlaylist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistItems_Episodes_EpisodeId",
                table: "PlaylistItems");

            migrationBuilder.DropIndex(
                name: "IX_PlaylistItems_EpisodeId",
                table: "PlaylistItems");

            migrationBuilder.DropColumn(
                name: "EpisodeId",
                table: "PlaylistItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EpisodeId",
                table: "PlaylistItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_PlaylistItems_EpisodeId",
                table: "PlaylistItems",
                column: "EpisodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistItems_Episodes_EpisodeId",
                table: "PlaylistItems",
                column: "EpisodeId",
                principalTable: "Episodes",
                principalColumn: "EpisodeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
