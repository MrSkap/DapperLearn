using Microsoft.EntityFrameworkCore;

namespace DapperStudy.Infrastructure.Auth.Migrator;

public class EfDbMigrator : IDbMigrator
{
    private readonly AuthDbContext _context;

    public EfDbMigrator(AuthDbContext context)
    {
        _context = context;
    }

    public async Task MigrateAsync()
    {
        await _context.Database.MigrateAsync();
    }
}