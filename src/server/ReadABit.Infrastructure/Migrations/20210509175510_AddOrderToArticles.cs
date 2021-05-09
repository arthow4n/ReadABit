using Microsoft.EntityFrameworkCore.Migrations;

namespace ReadABit.Infrastructure.Migrations
{
    public partial class AddOrderToArticles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Articles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(
@"
WITH ordered AS (
  SELECT a.""Id"", DENSE_RANK() OVER (PARTITION BY a.""ArticleCollectionId"" ORDER BY a.""CreatedAt"") AS i
  FROM ""Articles"" a
)
UPDATE ""Articles""
SET ""Order"" = ordered.i
FROM ordered
WHERE ""Articles"".""Id"" = ordered.""Id"";
"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "Articles");
        }
    }
}
