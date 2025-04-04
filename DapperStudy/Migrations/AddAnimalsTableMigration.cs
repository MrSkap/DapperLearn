using FluentMigrator;

namespace DapperStudy.Migrations;

[Migration(1)]
public class AddAnimalsTableMigration : Migration
{
    public override void Up()
    {
        Create
            .Table("animals")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("name").AsString(100).NotNullable()
            .WithColumn("age").AsInt32().NotNullable()
            .WithColumn("weight").AsDouble().Nullable();
    }

    public override void Down()
    {
        Delete.Table("animals");
    }
}