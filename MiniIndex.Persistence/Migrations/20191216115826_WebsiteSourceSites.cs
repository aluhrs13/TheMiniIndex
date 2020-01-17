using Microsoft.EntityFrameworkCore.Migrations;

namespace MiniIndex.Migrations
{
    public partial class WebsiteSourceSites : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WebsiteProfilePageURL",
                table: "SourceSite",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WebsiteURL",
                table: "SourceSite",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE	Creator
	                SET	WebsiteURL = NULL
                WHERE	TRIM(WebsiteURL) = ''

                UPDATE	Creator 
                    SET	WebsiteURL = 
	                CASE
		                WHEN CHARINDEX('/', REVERSE(WebsiteURL) + '/') = 1
		                THEN LEFT(WebsiteURL, LEN(WebsiteURL) - CHARINDEX('/', REVERSE(WebsiteURL) + '/'))
		                ELSE WebsiteURL
	                END
                WHERE	WebsiteURL IS NOT NULL

                INSERT INTO SourceSite
                (
	                SiteName,
	                CreatorID,
	                WebsiteURL
                )
                SELECT
		                'Website' AS SiteName,
		                c.ID AS CreatorId,
		                WebsiteURL AS WebsiteURL
                FROM	Creator c
                WHERE	c.WebsiteURL IS NOT NULL
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WebsiteProfilePageURL",
                table: "SourceSite");

            migrationBuilder.DropColumn(
                name: "WebsiteURL",
                table: "SourceSite");
        }
    }
}
