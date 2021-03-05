using Microsoft.EntityFrameworkCore.Migrations;

namespace ReadABit.Infrastructure.Migrations
{
    public partial class AddLanguageCodeToArticleCollectionAndRenameArticleColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Articles",
                newName: "Text");

            migrationBuilder.AddColumn<string>(
                name: "Conllu",
                table: "Articles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Articles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LanguageCode",
                table: "ArticleCollections",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Conllu",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "LanguageCode",
                table: "ArticleCollections");

            migrationBuilder.RenameColumn(
                name: "Text",
                table: "Articles",
                newName: "Title");
        }
    }
}
