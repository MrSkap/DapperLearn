using DapperStudy.Migrations.Extensions;
using FluentMigrator.Runner;

namespace DapperStudy.Migrations.Runner;

public static class MigrationsRunner
{
    public static void RunMigrations(IConfiguration configuration)
    {
        var serviceProvider = new ServiceCollection()
            .AddPostgresMigrations(configuration)
            .BuildServiceProvider(false);
        using var serviceScope = serviceProvider.CreateScope();

        serviceProvider.GetRequiredService<IMigrationRunner>().MigrateUp();
    }
}