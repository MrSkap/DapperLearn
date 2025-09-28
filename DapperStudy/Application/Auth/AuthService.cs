using System.Security.Cryptography;
using System.Text;
using DapperStudy.Infrastructure.Auth;
using DapperStudy.Infrastructure.Auth.Entities;
using DapperStudy.Models.Models;
using DapperStudy.Models.User;
using Microsoft.EntityFrameworkCore;

namespace DapperStudy.Application.Auth;

/// <inheritdoc />
public class AuthService : IAuthService
{
    private readonly AuthDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly IUserRepository _userRepository;

    public AuthService(IJwtService jwtService, IUserRepository userRepository, AuthDbContext context)
    {
        _jwtService = jwtService;
        _userRepository = userRepository;
        _context = context;
    }

    /// <inheritdoc />
    public async Task<string> Register(RegisterRequest request)
    {
        if (await _userRepository.UserExistsAsync(request.Username, request.Email))
            throw new Exception("User with this username or email already exists");

        var availableRoles = await GetAvailableRolesAsync();
        var invalidRoles = request.Roles.Except(availableRoles).ToList();
        if (invalidRoles.Any())
            throw new Exception($"Invalid roles: {string.Join(", ", invalidRoles)}");

        var user = new UserEntity
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.CreateUserAsync(user);

        await _userRepository.AssignRolesToUserAsync(user.Id, request.Roles);

        var createdUser = await _userRepository.GetUserByIdAsync(user.Id);
        return _jwtService.GenerateToken(MapFromEntity(createdUser!));
    }

    /// <inheritdoc />
    public async Task<string?> Authenticate(LoginRequest request)
    {
        var user = await _userRepository.GetUserByUsernameAsync(request.Username);
        if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
            return null;
        return _jwtService.GenerateToken(MapFromEntity(user));
    }

    /// <inheritdoc />
    public bool VerifyPassword(string password, string passwordHash)
    {
        return HashPassword(password) == passwordHash;
    }

    private async Task<List<string>> GetAvailableRolesAsync()
    {
        return await _context.Roles.Select(r => r.Name).ToListAsync();
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private User MapFromEntity(UserEntity entity)
    {
        return new User
        {
            Id = entity.Id,
            Username = entity.Username,
            Email = entity.Email,
            PasswordHash = entity.PasswordHash,
            Roles = entity.UserRoles.Select(x => x.Role.Name).ToArray()
        };
    }
}