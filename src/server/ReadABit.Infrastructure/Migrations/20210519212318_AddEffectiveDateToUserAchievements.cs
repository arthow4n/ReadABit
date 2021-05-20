using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

namespace ReadABit.Infrastructure.Migrations
{
    public partial class AddEffectiveDateToUserAchievements : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<LocalDate>(
                name: "EffectiveDate",
                table: "UserAchievements",
                type: "date",
                nullable: false,
                defaultValue: new NodaTime.LocalDate(1, 1, 1));

            migrationBuilder.Sql($@"
                WITH r AS (
                    SELECT ""Id"", ""CreatedAt"", ""EffectiveDate""
                    FROM ""UserAchievements"" ua
                    WHERE ua.""EffectiveDate"" = '0001-01-01'
                )
                UPDATE ""UserAchievements"" ua
                SET ""EffectiveDate"" = r.""CreatedAt""::date
                FROM r
                WHERE ua.""Id"" = r.""Id"";
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EffectiveDate",
                table: "UserAchievements");
        }
    }
}
