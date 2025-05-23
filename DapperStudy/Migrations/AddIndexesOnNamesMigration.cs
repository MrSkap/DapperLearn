using FluentMigrator;

namespace DapperStudy.Migrations;

[Migration(5)]
public class AddIndexesOnNamesMigration : Migration
{
    public override void Up()
    {
        Execute.Sql(@"
            CREATE INDEX index_animal_name ON animals (name);
        ");

        Execute.Sql(@"
            CREATE UNIQUE INDEX index_aviary_name ON aviaries (name);
        ");
    }

    public override void Down()
    {
        Execute.Sql("DROP INDEX index_animal_name");

        Execute.Sql("DROP INDEX index_aviary_name");
    }
}