using Microsoft.EntityFrameworkCore.Migrations;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Infrastructure.Migrations
{
    public partial class DropEavUserPreferencesAndReplaceWithJson : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "UserPreferences");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "UserPreferences");

            migrationBuilder.AddColumn<UserPreferenceData>(
                name: "Data",
                table: "UserPreferences",
                type: "jsonb",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferences_UserId",
                table: "UserPreferences",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPreferences_AspNetUsers_UserId",
                table: "UserPreferences",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPreferences_AspNetUsers_UserId",
                table: "UserPreferences");

            migrationBuilder.DropIndex(
                name: "IX_UserPreferences_UserId",
                table: "UserPreferences");

            migrationBuilder.DropColumn(
                name: "Data",
                table: "UserPreferences");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "UserPreferences",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "UserPreferences",
                type: "text",
                nullable: true);
        }
    }
}
