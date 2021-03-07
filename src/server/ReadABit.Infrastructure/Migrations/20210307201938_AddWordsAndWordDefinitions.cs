using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

namespace ReadABit.Infrastructure.Migrations
{
    public partial class AddWordsAndWordDefinitions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Words",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LanguageCode = table.Column<string>(type: "text", nullable: false),
                    Expression = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Words", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WordDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WordId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Public = table.Column<bool>(type: "boolean", nullable: false),
                    LanguageCode = table.Column<string>(type: "text", nullable: false),
                    Meaning = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<Instant>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WordDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WordDefinitions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WordDefinitions_Words_WordId",
                        column: x => x.WordId,
                        principalTable: "Words",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleCollections_LanguageCode_Name",
                table: "ArticleCollections",
                columns: new[] { "LanguageCode", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_WordDefinitions_LanguageCode",
                table: "WordDefinitions",
                column: "LanguageCode");

            migrationBuilder.CreateIndex(
                name: "IX_WordDefinitions_UserId",
                table: "WordDefinitions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WordDefinitions_WordId",
                table: "WordDefinitions",
                column: "WordId");

            migrationBuilder.CreateIndex(
                name: "IX_Words_LanguageCode_Expression",
                table: "Words",
                columns: new[] { "LanguageCode", "Expression" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WordDefinitions");

            migrationBuilder.DropTable(
                name: "Words");

            migrationBuilder.DropIndex(
                name: "IX_ArticleCollections_LanguageCode_Name",
                table: "ArticleCollections");
        }
    }
}
