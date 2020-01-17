using Microsoft.EntityFrameworkCore.Migrations;

namespace MiniIndex.Migrations
{
    public partial class InsertMissingGumroadSources : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO MiniSourceSite
				(
					MiniID,
					SiteID,
					Link
				)
				SELECT		
							m.ID as MiniID,
							ss.ID as SiteID,
							m.Link as Link

				FROM		Mini m
					JOIN	Creator c
						ON	m.CreatorID = c.ID
					JOIN	SourceSite ss
						ON	ss.CreatorID = c.ID
					LEFT OUTER JOIN	MiniSourceSite ms
						ON	ms.SiteID = ss.ID

				WHERE		m.Link LIKE '%gumroad.com%'
					AND		ss.SiteName = 'Gumroad'
					AND		ms.Link IS NULL
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
