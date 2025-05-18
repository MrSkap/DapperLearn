using FluentMigrator;

namespace DapperStudy.Migrations;

[Migration(4)]
public class AddAviarySummaryViewMigration : Migration
{
    public override void Up()
    {
        Execute.Sql(@"
            CREATE OR REPLACE FUNCTION GetAviarySummary()
            RETURNS TABLE (
                id uuid,
                name text,
                type text,
                animalsCount integer,
                width double precision,
                height double precision,
                depth double precision
            ) 
            LANGUAGE plpgsql
            STABLE
            AS $$
            BEGIN
                RETURN QUERY
                SELECT 
                    a.id,
                    a.name,
                    avs.type,
                    COALESCE(animals.animalsCount, 0)::integer as animalsCount,
                    avs.width,
                    avs.height,
                    avs.depth
                FROM 
                    aviaries a
                LEFT JOIN aviary_settings avs
                    ON a.""settingsId"" = avs.id
                LEFT JOIN
                    (SELECT
                        ""aviaryId"",
                        COUNT(*) as animalsCount
                    FROM animals GROUP BY ""aviaryId"") as animals
                    ON a.id = animals.""aviaryId""
                ORDER BY 
                    a.name;
            END;
            $$;
        ");

        Execute.Sql(@"
                CREATE OR REPLACE VIEW aviary_summary_view AS
                SELECT * FROM GetAviarySummary();
            ");
    }

    public override void Down()
    {
        Execute.Sql("DROP VIEW IF EXISTS aviary_summary_view;");

        Execute.Sql("DROP FUNCTION IF EXISTS GetAviarySummary();");
    }
}