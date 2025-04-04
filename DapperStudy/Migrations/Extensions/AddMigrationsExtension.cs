using FluentMigrator.Runner;

namespace DapperStudy.Migrations.Extensions;

public static class AddMigrationsExtension
{
    public static IServiceCollection AddPostgresMigrations(this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services
            .AddFluentMigratorCore()
            .ConfigureRunner(runnerBuilder =>
                runnerBuilder
                    .AddPostgres()
                    .WithGlobalConnectionString(connectionString)
                    .ScanIn(typeof(AddAnimalsTableMigration).Assembly).For.Migrations())
            .AddLogging(loggingBuilder => loggingBuilder.AddFluentMigratorConsole());

        return services;
    }
}