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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EffectiveDate",
                table: "UserAchievements");
        }
    }
}
