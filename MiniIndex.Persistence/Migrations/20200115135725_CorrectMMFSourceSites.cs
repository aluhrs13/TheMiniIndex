using Microsoft.EntityFrameworkCore.Migrations;

namespace MiniIndex.Migrations
{
    public partial class CorrectMMFSourceSites : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE	SourceSite
                SET		SiteName = 'MyMiniFactory',
		                CreatorUserName = RIGHT(WebsiteURL, LEN(WebsiteURL) - (PATINDEX('%users%', WebsiteURL) + 5)),
		                WebsiteUrl = NULL
                WHERE	WebsiteURL LIKE '%myminifactory.com%'
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
