using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

namespace ReadABit.Infrastructure.Migrations
{
    public partial class AddTimestampsToWordFamiliarity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Instant>(
                name: "CreatedAt",
                table: "WordFamiliarities",
                type: "timestamp",
                nullable: false,
                defaultValue: NodaTime.Instant.FromUnixTimeTicks(0L));

            migrationBuilder.AddColumn<Instant>(
                name: "UpdatedAt",
                table: "WordFamiliarities",
                type: "timestamp",
                nullable: false,
                defaultValue: NodaTime.Instant.FromUnixTimeTicks(0L));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "WordFamiliarities");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "WordFamiliarities");
        }
    }
}
