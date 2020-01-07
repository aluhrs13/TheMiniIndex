using Microsoft.EntityFrameworkCore.Migrations;

namespace MiniIndex.Migrations
{
    public partial class RemoveUrlColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PatreonURL",
                table: "Creator");

            migrationBuilder.DropColumn(
                name: "ShapewaysURL",
                table: "Creator");

            migrationBuilder.DropColumn(
                name: "ThingiverseURL",
                table: "Creator");

            migrationBuilder.DropColumn(
                name: "WebsiteURL",
                table: "Creator");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PatreonURL",
                table: "Creator",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShapewaysURL",
                table: "Creator",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThingiverseURL",
                table: "Creator",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WebsiteURL",
                table: "Creator",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}