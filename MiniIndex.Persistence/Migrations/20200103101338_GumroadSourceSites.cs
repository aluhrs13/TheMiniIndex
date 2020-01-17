using Microsoft.EntityFrameworkCore.Migrations;

namespace MiniIndex.Migrations
{
    public partial class GumroadSourceSites : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GumroadUsername",
                table: "SourceSite",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GumroadUsername",
                table: "SourceSite");
        }
    }
}
