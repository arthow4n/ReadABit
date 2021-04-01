using Microsoft.EntityFrameworkCore.Migrations;

namespace ReadABit.Infrastructure.Migrations
{
    public partial class DeferFkConstraintOnWordFamiliarity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Making this deferred because we run batch upsert inside a transaction.
            migrationBuilder.Sql(@"
            ALTER TABLE ""WordFamiliarities""
            ALTER CONSTRAINT ""FK_WordFamiliarities_Words_WordId"" DEFERRABLE INITIALLY DEFERRED;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            ALTER TABLE ""WordFamiliarities""
            ALTER CONSTRAINT ""FK_WordFamiliarities_Words_WordId"" NOT DEFERRABLE;
            ");
        }
    }
}
