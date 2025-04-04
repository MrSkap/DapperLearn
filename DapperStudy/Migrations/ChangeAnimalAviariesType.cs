using FluentMigrator;

namespace DapperStudy.Migrations;

[Migration(3)]
public class ChangeAnimalAviariesType : Migration
{
    public override void Up()
    {
        Alter.Table("animals").AlterColumn("aviaryId").AsGuid().Nullable();
    }

    public override void Down()
    {
    }
}