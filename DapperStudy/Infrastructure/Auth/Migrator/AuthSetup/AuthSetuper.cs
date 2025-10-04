using System.Security.Cryptography;
using System.Text;
using DapperStudy.Configuration;
using DapperStudy.Infrastructure.Auth.Entities;
using DapperStudy.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace DapperStudy.Infrastructure.Auth.Migrator.AuthSetup;

public class AuthSetuper : IAuthSetuper
{
    private readonly DefaultAdminConfiguration _adminConfiguration;

    public AuthSetuper(DefaultAdminConfiguration adminConfiguration)
    {
        _adminConfiguration = adminConfiguration;
    }

    /// <inheritdoc />
    public async Task SeedDataAsync(AuthDbContext context)
    {
        if (!await context.Roles.AnyAsync())
        {
            await context.Roles.AddRangeAsync(
                new Role { Id = Guid.NewGuid(), Name = KnownRoles.Viewer, Description = "Basic user role" },
                new Role { Id = Guid.NewGuid(), Name = KnownRoles.Admin, Description = "Administrator role" },
                new Role { Id = Guid.NewGuid(), Name = KnownRoles.Worker, Description = "Animal worker role" },
                new Role { Id = Guid.NewGuid(), Name = KnownRoles.Manager, Description = "Manager role" }
            );
            await context.SaveChangesAsync();
        }

        if (!await context.Users.AnyAsync(u => u.Username == "admin"))
        {
            var adminUser = new UserEntity
            {
                Id = Guid.Parse(_adminConfiguration.Id),
                Username = _adminConfiguration.Name,
                Email = "admin@admin.com",
                PasswordHash = HashPassword(_adminConfiguration.Password)
            };

            await context.Users.AddAsync(adminUser);
            await context.SaveChangesAsync();
            var viewerRoleId = (await context.Roles.FirstAsync(x => x.Name == KnownRoles.Viewer)).Id;
            var adminRoleId = (await context.Roles.FirstAsync(x => x.Name == KnownRoles.Admin)).Id;
            await context.UserRoles.AddRangeAsync(
                new UserRole { UserId = adminUser.Id, RoleId = viewerRoleId },
                new UserRole { UserId = adminUser.Id, RoleId = adminRoleId }
            );
            await context.SaveChangesAsync();
        }
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}