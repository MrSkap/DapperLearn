namespace DapperStudy.Models.Models;

public record ProfileResponse(
    string? UserId,
    string? Username,
    string? Email,
    string? Role);