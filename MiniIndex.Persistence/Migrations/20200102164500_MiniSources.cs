using Microsoft.EntityFrameworkCore.Migrations;

namespace MiniIndex.Migrations
{
    public partial class MiniSources : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MiniSourceSite",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MiniID = table.Column<int>(nullable: true),
                    SiteID = table.Column<int>(nullable: true),
                    Link = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MiniSourceSite", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MiniSourceSite_Mini_MiniID",
                        column: x => x.MiniID,
                        principalTable: "Mini",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MiniSourceSite_SourceSite_SiteID",
                        column: x => x.SiteID,
                        principalTable: "SourceSite",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MiniSourceSite_MiniID",
                table: "MiniSourceSite",
                column: "MiniID");

            migrationBuilder.CreateIndex(
                name: "IX_MiniSourceSite_SiteID",
                table: "MiniSourceSite",
                column: "SiteID");

            migrationBuilder.Sql(@"
                INSERT INTO MiniSourceSite
                SELECT
	                m.ID,
	                s.ID,
	                m.Link	
                FROM	Mini m
                JOIN	SourceSite s
	                ON	s.CreatorID = m.CreatorID
                WHERE	s.SiteName = 'Thingiverse'
	                AND	m.Link LIKE '%thingiverse.com%'

                INSERT INTO MiniSourceSite
                SELECT	m.ID,
		                s.ID,
		                m.Link
                FROM	Mini m
                JOIN	SourceSite s
	                ON	s.CreatorID = m.CreatorID
                WHERE	s.SiteName = 'Shapeways'
	                AND	m.Link LIKE '%shapeways.com%'

                INSERT INTO MiniSourceSite
                SELECT	m.ID,
		                s.ID,
		                m.Link
                FROM	Mini m
                JOIN	SourceSite s
	                ON	s.CreatorID = m.CreatorID
                WHERE	s.SiteName = 'Patreon'
	                AND	m.Link LIKE '%patreon.com%'
	
                INSERT INTO MiniSourceSite
                SELECT	m.ID,
		                s.ID,
		                m.Link
                FROM	Mini m
                JOIN	SourceSite s
	                ON	s.CreatorID = m.CreatorID
                WHERE	s.SiteName = 'Gumroad'
	                AND	m.Link LIKE '%gumroad.com%'
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MiniSourceSite");
        }
    }
}
