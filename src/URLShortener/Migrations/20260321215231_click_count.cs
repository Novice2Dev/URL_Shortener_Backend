using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace URL_Shortener_Backend.Migrations
{
    /// <inheritdoc />
    public partial class click_count : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClickCount",
                table: "UrlMappings",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClickCount",
                table: "UrlMappings");
        }
    }
}
