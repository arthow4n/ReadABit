using Microsoft.EntityFrameworkCore.Migrations;

namespace ReadABit.Infrastructure.Migrations
{
    public partial class ChangeArticleAnnotationSchemeToSparvXml : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Conllu",
                table: "Articles",
                newName: "SparvXmlVersion");

            migrationBuilder.AddColumn<string>(
                name: "SparvXmlJson",
                table: "Articles",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SparvXmlJson",
                table: "Articles");

            migrationBuilder.RenameColumn(
                name: "SparvXmlVersion",
                table: "Articles",
                newName: "Conllu");
        }
    }
}
