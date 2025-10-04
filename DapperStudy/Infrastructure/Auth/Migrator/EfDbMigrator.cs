using Microsoft.EntityFrameworkCore;
using Serilog;
using ILogger = Serilog.ILogger;

namespace DapperStudy.Infrastructure.Auth.Migrator;

public class EfDbMigrator : IDbMigrator
{
    private static readonly ILogger Logger = Log.ForContext<EfDbMigrator>();
    private readonly AuthDbContext _context;

    public EfDbMigrator(AuthDbContext context)
    {
        _context = context;
    }

    public async Task MigrateAsync()
    {
        if (await HasPendingMigrationsAsync()) await _context.Database.MigrateAsync();
    }

    private async Task<bool> HasPendingMigrationsAsync()
    {
        try
        {
            var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
            return pendingMigrations.Any();
        }
        catch (Exception ex)
        {
            Logger.Error($"Error checking pending migrations: {ex.Message}");
            return false;
        }
    }
}