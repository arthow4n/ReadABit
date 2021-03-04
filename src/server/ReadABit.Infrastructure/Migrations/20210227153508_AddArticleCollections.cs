using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ReadABit.Infrastructure.Migrations
{
    public partial class AddArticleCollections : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ArticleCollectionId",
                table: "Articles",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "ArticleCollections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleCollections", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Articles_ArticleCollectionId",
                table: "Articles",
                column: "ArticleCollectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_ArticleCollections_ArticleCollectionId",
                table: "Articles",
                column: "ArticleCollectionId",
                principalTable: "ArticleCollections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_ArticleCollections_ArticleCollectionId",
                table: "Articles");

            migrationBuilder.DropTable(
                name: "ArticleCollections");

            migrationBuilder.DropIndex(
                name: "IX_Articles_ArticleCollectionId",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "ArticleCollectionId",
                table: "Articles");
        }
    }
}
