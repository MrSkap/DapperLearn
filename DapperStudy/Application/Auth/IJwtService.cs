using DapperStudy.Models.User;

namespace DapperStudy.Application.Auth;

/// <summary>
/// Сервис генерации JWT токенов.
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Сгенерировать токен.
    /// </summary>
    /// <param name="user">Пользователь.</param>
    /// <returns>Токен.</returns>
    string GenerateToken(User user);
}