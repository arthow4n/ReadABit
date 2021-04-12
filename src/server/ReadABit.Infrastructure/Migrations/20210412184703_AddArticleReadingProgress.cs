using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using ReadABit.Core.Integrations.Contracts.Conllu;

namespace ReadABit.Infrastructure.Migrations
{
    public partial class AddArticleReadingProgress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArticleReadingProgress",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ArticleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConlluTokenPointer = table.Column<Conllu.TokenPointer>(type: "jsonb", nullable: false),
                    ReadRatio = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedAt = table.Column<Instant>(type: "timestamp", nullable: false),
                    UpdatedAt = table.Column<Instant>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleReadingProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArticleReadingProgress_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArticleReadingProgress_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleReadingProgress_ArticleId",
                table: "ArticleReadingProgress",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleReadingProgress_UserId_ArticleId",
                table: "ArticleReadingProgress",
                columns: new[] { "UserId", "ArticleId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticleReadingProgress");
        }
    }
}
