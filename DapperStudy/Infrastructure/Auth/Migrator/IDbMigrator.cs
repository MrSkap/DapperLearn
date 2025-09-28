namespace DapperStudy.Infrastructure.Auth.Migrator;

public interface IDbMigrator
{
    Task MigrateAsync();
}