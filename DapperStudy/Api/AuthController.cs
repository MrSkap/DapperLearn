using System.Security.Claims;
using DapperStudy.Api.Filters;
using DapperStudy.Application.Auth;
using DapperStudy.Infrastructure.Auth;
using DapperStudy.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KnownRoles = DapperStudy.Models.User.KnownRoles;

namespace DapperStudy.Api;

[ApiController]
[Route("api/auth")]
[ServiceFilter(typeof(BadResponseFilter))]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserRepository _userRepository;

    public AuthController(IAuthService authService, IUserRepository userRepository)
    {
        _authService = authService;
        _userRepository = userRepository;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var token = await _authService.Register(request);

            return Ok(new AuthResponse
            {
                Token = token,
                Expires = DateTime.UtcNow.AddHours(24)
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var token = await _authService.Authenticate(request);
        if (string.IsNullOrEmpty(token))
            return Unauthorized(new { message = "Invalid credentials" });

        return Ok(new AuthResponse
        {
            Token = token,
            Expires = DateTime.UtcNow.AddHours(24)
        });
    }

    [Authorize]
    [HttpGet("profile")]
    public ProfileResponse GetProfile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var username = User.FindFirst(ClaimTypes.Name)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        return new ProfileResponse(
            userId,
            username,
            email,
            role
        );
    }

    [Authorize(Roles = KnownRoles.Admin)]
    [HttpGet]
    [ServiceFilter(typeof(AdminFilter))]
    public async Task<IEnumerable<UserResponse>?> GetAllUsers()
    {
        var users = await _userRepository.GetAllUsersAsync();
        var response = users.Select(u => new UserResponse
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList(),
            CreatedAt = u.CreatedAt
        });

        return response;
    }

    [Authorize(Roles = KnownRoles.Admin)]
    [HttpPost("{userId:guid}/roles")]
    [ServiceFilter(typeof(AdminFilter))]
    public async Task<IActionResult> AssignRoles(Guid userId, [FromBody] List<string> roles)
    {
        await _userRepository.AssignRolesToUserAsync(userId, roles);
        return Ok(new { message = "Roles assigned successfully" });
    }
}