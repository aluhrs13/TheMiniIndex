using Microsoft.EntityFrameworkCore.Migrations;

namespace MiniIndex.Migrations
{
    public partial class CorrectGumroadSourceSites : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE	SourceSite
                SET		SiteName = 'Gumroad',
		                CreatorUserName = RIGHT(WebsiteURL, LEN(WebsiteURL) - (PATINDEX('%users%', WebsiteURL) + 20)),
		                WebsiteUrl = NULL
                WHERE	WebsiteURL LIKE '%gumroad.com%'
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
