using Microsoft.EntityFrameworkCore.Migrations;
using ReadABit.Core.Integrations.Contracts.Conllu;

namespace ReadABit.Infrastructure.Migrations
{
    public partial class AddConlluDocument : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Conllu",
                table: "Articles");

            migrationBuilder.AddColumn<Conllu.Document>(
                name: "ConlluDocument",
                table: "Articles",
                type: "jsonb",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConlluDocument",
                table: "Articles");

            migrationBuilder.AddColumn<string>(
                name: "Conllu",
                table: "Articles",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
