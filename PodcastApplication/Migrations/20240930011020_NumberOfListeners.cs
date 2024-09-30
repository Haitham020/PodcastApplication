using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PodcastApplication.Migrations
{
    /// <inheritdoc />
    public partial class NumberOfListeners : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ListenerNumber",
                table: "Episodes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ListenerNumber",
                table: "Episodes");
        }
    }
}
