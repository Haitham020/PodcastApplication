using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PodcastApplication.Migrations
{
    /// <inheritdoc />
    public partial class ModifyPlaylistModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PlaylistName",
                table: "Playlists",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Playlists",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PlaylistImg",
                table: "Playlists",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Playlists");

            migrationBuilder.DropColumn(
                name: "PlaylistImg",
                table: "Playlists");

            migrationBuilder.AlterColumn<string>(
                name: "PlaylistName",
                table: "Playlists",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
