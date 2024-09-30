using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PodcastApplication.Migrations
{
    /// <inheritdoc />
    public partial class PodcastCreator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatorId",
                table: "Podcasts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatorGenre",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Podcasts_CreatorId",
                table: "Podcasts",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Podcasts_AspNetUsers_CreatorId",
                table: "Podcasts",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Podcasts_AspNetUsers_CreatorId",
                table: "Podcasts");

            migrationBuilder.DropIndex(
                name: "IX_Podcasts_CreatorId",
                table: "Podcasts");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Podcasts");

            migrationBuilder.DropColumn(
                name: "CreatorGenre",
                table: "AspNetUsers");
        }
    }
}
