using Microsoft.EntityFrameworkCore.Migrations;

namespace MiniIndex.Migrations
{
    public partial class Starred : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PatreonURL",
                table: "Creator");

            migrationBuilder.DropColumn(
                name: "ShapewaysURL",
                table: "Creator");

            migrationBuilder.DropColumn(
                name: "ThingiverseURL",
                table: "Creator");

            migrationBuilder.DropColumn(
                name: "WebsiteURL",
                table: "Creator");

            migrationBuilder.CreateTable(
                name: "Starred",
                columns: table => new
                {
                    MiniID = table.Column<int>(nullable: false),
                    UserID = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Starred", x => new { x.MiniID, x.UserID });
                    table.ForeignKey(
                        name: "FK_Starred_Mini_MiniID",
                        column: x => x.MiniID,
                        principalTable: "Mini",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Starred_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Starred_UserID",
                table: "Starred",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Starred");

            migrationBuilder.AddColumn<string>(
                name: "PatreonURL",
                table: "Creator",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShapewaysURL",
                table: "Creator",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThingiverseURL",
                table: "Creator",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WebsiteURL",
                table: "Creator",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
