using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MiniIndex.Migrations
{
    public partial class TimeStampAndUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TaggerId",
                table: "MiniTag",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedTime",
                table: "Mini",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_MiniTag_TaggerId",
                table: "MiniTag",
                column: "TaggerId");

            migrationBuilder.AddForeignKey(
                name: "FK_MiniTag_AspNetUsers_TaggerId",
                table: "MiniTag",
                column: "TaggerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MiniTag_AspNetUsers_TaggerId",
                table: "MiniTag");

            migrationBuilder.DropIndex(
                name: "IX_MiniTag_TaggerId",
                table: "MiniTag");

            migrationBuilder.DropColumn(
                name: "TaggerId",
                table: "MiniTag");

            migrationBuilder.DropColumn(
                name: "ApprovedTime",
                table: "Mini");
        }
    }
}
