using DapperStudy.Infrastructure.Auth.Entities;
using Microsoft.EntityFrameworkCore;

namespace DapperStudy.Infrastructure.Auth;

/// <inheritdoc />
public class UserRepository : IUserRepository
{
    private readonly AuthDbContext _context;

    public UserRepository(AuthDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<UserEntity?> GetUserByIdAsync(Guid id)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    /// <inheritdoc />
    public async Task<UserEntity?> GetUserByUsernameAsync(string username)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    /// <inheritdoc />
    public async Task<UserEntity?> GetUserByEmailAsync(string email)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    /// <inheritdoc />
    public async Task<List<UserEntity>> GetAllUsersAsync()
    {
        return await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task CreateUserAsync(UserEntity userEntity)
    {
        await _context.Users.AddAsync(userEntity);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task UpdateUserAsync(UserEntity userEntity)
    {
        userEntity.UpdatedAt = DateTime.UtcNow;
        _context.Users.Update(userEntity);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task<bool> UserExistsAsync(string username, string email)
    {
        return await _context.Users
            .AnyAsync(u => u.Username == username || u.Email == email);
    }

    /// <inheritdoc />
    public async Task<List<string>> GetUserRolesAsync(Guid userId)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Include(ur => ur.Role)
            .Select(ur => ur.Role.Name)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task AssignRolesToUserAsync(Guid userId, IEnumerable<string> roleNames)
    {
        var existingUserRoles = await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .ToListAsync();

        _context.UserRoles.RemoveRange(existingUserRoles);

        var roles = await _context.Roles
            .Where(r => roleNames.Contains(r.Name))
            .ToListAsync();

        var newUserRoles = roles.Select(role => new UserRole
        {
            UserId = userId,
            RoleId = role.Id,
            AssignedAt = DateTime.UtcNow
        });

        await _context.UserRoles.AddRangeAsync(newUserRoles);
        await _context.SaveChangesAsync();
    }
}