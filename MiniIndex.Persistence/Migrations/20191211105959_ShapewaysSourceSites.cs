using Microsoft.EntityFrameworkCore.Migrations;

namespace MiniIndex.Migrations
{
    public partial class ShapewaysSourceSites : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShapewaysUsername",
                table: "SourceSite",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE	Creator
	                SET	ShapewaysURL = NULL
                WHERE	TRIM(ShapewaysURL) = ''

                UPDATE	Creator
                    SET ShapewaysURL =
	                CASE
				        WHEN CHARINDEX('/', REVERSE(ShapewaysURL) + '/') = 1
				        THEN LEFT(ShapewaysURL, LEN(ShapewaysURL) - CHARINDEX('/', REVERSE(ShapewaysURL) + '/'))
				        ELSE ShapewaysURL
				    END
                WHERE	ShapewaysURL IS NOT NULL

                INSERT INTO SourceSite
                (
	                SiteName,
	                CreatorID,
	                ShapewaysUsername
                )
                SELECT
		                'Shapeways' AS SiteName,
		                c.ID AS CreatorId,
		                RIGHT(c.ShapewaysURL, CHARINDEX('/', REVERSE(c.ShapewaysURL) + '/') - 1) AS ShapewaysUsername
                FROM	Creator c
                WHERE	c.ShapewaysURL IS NOT NULL
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShapewaysUsername",
                table: "SourceSite");
        }
    }
}