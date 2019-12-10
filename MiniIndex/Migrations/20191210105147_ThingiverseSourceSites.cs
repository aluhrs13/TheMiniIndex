using Microsoft.EntityFrameworkCore.Migrations;

namespace MiniIndex.Migrations
{
    public partial class ThingiverseSourceSites : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SourceSite",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayName = table.Column<string>(nullable: false),
                    CreatorID = table.Column<int>(nullable: true),
                    ThingiverseUsername = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SourceSite", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SourceSite_Creator_CreatorID",
                        column: x => x.CreatorID,
                        principalTable: "Creator",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SourceSite_CreatorID",
                table: "SourceSite",
                column: "CreatorID");

            migrationBuilder.Sql(@"
                INSERT INTO SourceSite

                SELECT	
		                'Thingiverse' AS DisplayName,
		                c.ID AS CreatorId,
		                RIGHT(c.ThingiverseURL, CHARINDEX('/', REVERSE(c.ThingiverseURL) + '/') - 1) AS ThingiverseUsername

                FROM	Creator c

                WHERE	c.ThingiverseURL IS NOT NULL

                SELECT * FROM SourceSite
                ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SourceSite");
        }
    }
}
