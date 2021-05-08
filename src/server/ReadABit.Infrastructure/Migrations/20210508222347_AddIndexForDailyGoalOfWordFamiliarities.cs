using Microsoft.EntityFrameworkCore.Migrations;

namespace ReadABit.Infrastructure.Migrations
{
    public partial class AddIndexForDailyGoalOfWordFamiliarities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_WordFamiliarities_UserId_CreatedAt_Level",
                table: "WordFamiliarities",
                columns: new[] { "UserId", "CreatedAt", "Level" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WordFamiliarities_UserId_CreatedAt_Level",
                table: "WordFamiliarities");
        }
    }
}
