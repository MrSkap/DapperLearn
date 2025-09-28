using DapperStudy.Models.Models;

namespace DapperStudy.Application.Auth;

/// <summary>
/// Сервис аутентификации.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Зарегестирировать нового пользователя.
    /// </summary>
    /// <param name="request">Запрос на создание нового пользователя.</param>
    /// <returns>Токен.</returns>
    Task<string> Register(RegisterRequest request);
    
    /// <summary>
    /// Авторизовать.
    /// </summary>
    /// <param name="request">Запрос на авторизацию.</param>
    /// <returns>Токен.</returns>
    Task<string?> Authenticate(LoginRequest request);
    
    /// <summary>
    /// Проверить сопадет ли хеш пароля с хешем хранимого пароля.
    /// </summary>
    /// <param name="password">Введенный пароль.</param>
    /// <param name="passwordHash">Хранимый хеш.</param>
    /// <returns>Совпадает или нет.</returns>
    bool VerifyPassword(string password, string passwordHash);
}