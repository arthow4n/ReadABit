using Microsoft.EntityFrameworkCore.Migrations;

namespace ReadABit.Infrastructure.Migrations
{
    public partial class AddPublicPropertyToArticleCollections : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Public",
                table: "ArticleCollections",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Public",
                table: "ArticleCollections");
        }
    }
}
