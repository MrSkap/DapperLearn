using FluentMigrator;

namespace DapperStudy.Migrations;

[Migration(2)]
public class AddAviaryMigration : Migration
{
    public override void Up()
    {
        Create
            .Table("aviaries")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("name").AsString().NotNullable().Unique()
            .WithColumn("settingsId").AsGuid().NotNullable();
        Create.Table("aviary_settings")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("type").AsString().NotNullable()
            .WithColumn("width").AsDouble().Nullable()
            .WithColumn("height").AsDouble().Nullable()
            .WithColumn("depth").AsDouble().Nullable();

        Create
            .ForeignKey("fk_aviary_to_settings").FromTable("aviaries").ForeignColumn("settingsId")
            .ToTable("aviary_settings").PrimaryColumn("id");

        var settingsId = Guid.NewGuid();
        var aviaryId = Guid.NewGuid();
        Insert.IntoTable("aviary_settings").Row(new { type = "Default Settings", id = settingsId });
        Insert.IntoTable("aviaries").Row(new { name = "Default Aviary", id = aviaryId, settingsId });

        Create
            .Column("aviaryId").OnTable("animals").AsGuid().NotNullable().SetExistingRowsTo(aviaryId);
        Create
            .ForeignKey("fk_animal_to_aviary").FromTable("animals").ForeignColumn("aviaryId").ToTable("aviaries")
            .PrimaryColumn("id");
    }

    public override void Down()
    {
        Delete.Table("aviaries");
        Delete.Table("aviary-settings");
    }
}