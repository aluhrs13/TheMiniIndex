using Microsoft.EntityFrameworkCore.Migrations;

namespace MiniIndex.Migrations
{
    public partial class PatreonSourceSites : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PatreonUsername",
                table: "SourceSite",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE	Creator
	                SET	PatreonURL = NULL
                WHERE	TRIM(PatreonURL) = ''

                UPDATE Creator
                SET PatreonURL = 
	                CASE
		                WHEN CHARINDEX('/', REVERSE(PatreonURL) + '/') = 1
		                THEN LEFT(PatreonURL, LEN(PatreonURL) - CHARINDEX('/', REVERSE(PatreonURL) + '/'))
		                ELSE PatreonURL
	                END
                FROM Creator
                WHERE	PatreonURL IS NOT NULL

                INSERT INTO SourceSite
                SELECT
		                'Patreon' AS SiteName,
                        c.ID AS CreatorId,
                        NULL AS ThingiverseUsername,
		                NULL AS ShapewaysUsername,
		                RIGHT(c.PatreonURL, CHARINDEX('/', REVERSE(c.PatreonURL) + '/') - 1) AS PatreonUsername
                FROM	Creator c
                WHERE	c.PatreonURL IS NOT NULL
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PatreonUsername",
                table: "SourceSite");
        }
    }
}