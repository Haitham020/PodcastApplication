using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PodcastApplication.Migrations
{
    /// <inheritdoc />
    public partial class delFrkCreaterCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Categories_CreatorCategoryId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CreatorCategoryId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CreatorCategoryId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "ProfileBio",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileBio",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "CreatorCategoryId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CreatorCategoryId",
                table: "AspNetUsers",
                column: "CreatorCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Categories_CreatorCategoryId",
                table: "AspNetUsers",
                column: "CreatorCategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId");
        }
    }
}
