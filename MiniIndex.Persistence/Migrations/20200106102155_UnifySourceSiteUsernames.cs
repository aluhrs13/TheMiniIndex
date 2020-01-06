using Microsoft.EntityFrameworkCore.Migrations;

namespace MiniIndex.Migrations
{
    public partial class UnifySourceSiteUsernames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatorUserName",
                table: "SourceSite",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE  SourceSite
                    SET CreatorUserName = COALESCE(ThingiverseUsername, ShapewaysUsername, PatreonUsername, GumroadUsername)
            ");

            migrationBuilder.DropColumn(
                name: "GumroadUsername",
                table: "SourceSite");

            migrationBuilder.DropColumn(
                name: "PatreonUsername",
                table: "SourceSite");

            migrationBuilder.DropColumn(
                name: "ShapewaysUsername",
                table: "SourceSite");

            migrationBuilder.DropColumn(
                name: "ThingiverseUsername",
                table: "SourceSite");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GumroadUsername",
                table: "SourceSite",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PatreonUsername",
                table: "SourceSite",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShapewaysUsername",
                table: "SourceSite",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThingiverseUsername",
                table: "SourceSite",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE		SourceSite
	                SET		ThingiverseUserName = CreatorUserName
	                WHERE	SiteName = 'Thingiverse'

                UPDATE		SourceSite
	                SET		ShapewaysUserName = CreatorUserName
	                WHERE	SiteName = 'Shapeways'

                UPDATE		SourceSite
	                SET		PatreonUserName = CreatorUserName
	                WHERE	SiteName = 'Patreon'

                UPDATE		SourceSite
	                SET		GumroadUserName = CreatorUserName
	                WHERE	SiteName = 'Gumroad'
            ");

            migrationBuilder.DropColumn(
                name: "CreatorUserName",
                table: "SourceSite");
        }
    }
}