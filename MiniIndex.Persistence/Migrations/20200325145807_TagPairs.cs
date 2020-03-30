using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MiniIndex.Migrations
{
    public partial class TagPairs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedTime",
                table: "MiniTag",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "MiniTag",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedTime",
                table: "MiniTag",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "TagPair",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tag1ID = table.Column<int>(nullable: true),
                    Tag2ID = table.Column<int>(nullable: true),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagPair", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TagPair_Tag_Tag1ID",
                        column: x => x.Tag1ID,
                        principalTable: "Tag",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TagPair_Tag_Tag2ID",
                        column: x => x.Tag2ID,
                        principalTable: "Tag",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TagPair_Tag1ID",
                table: "TagPair",
                column: "Tag1ID");

            migrationBuilder.CreateIndex(
                name: "IX_TagPair_Tag2ID",
                table: "TagPair",
                column: "Tag2ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TagPair");

            migrationBuilder.DropColumn(
                name: "ApprovedTime",
                table: "MiniTag");

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "MiniTag");

            migrationBuilder.DropColumn(
                name: "LastModifiedTime",
                table: "MiniTag");
        }
    }
}
