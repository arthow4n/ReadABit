using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ReadABit.Infrastructure.Migrations
{
    public partial class AddWordFamiliarity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WordFamiliarities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WordId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WordFamiliarities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WordFamiliarities_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WordFamiliarities_Words_WordId",
                        column: x => x.WordId,
                        principalTable: "Words",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WordFamiliarities_UserId",
                table: "WordFamiliarities",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WordFamiliarities_WordId",
                table: "WordFamiliarities",
                column: "WordId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WordFamiliarities");
        }
    }
}
