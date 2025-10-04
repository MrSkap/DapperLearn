using DapperStudy.Infrastructure.Auth.Entities;

namespace DapperStudy.Infrastructure.Auth;

/// <summary>
///     Репозиторий работы с пользователями.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    ///     Получить пользоватя по id.
    /// </summary>
    /// <param name="id">Идентификатор пользователя.</param>
    /// <returns><see cref="UserEntity" />.</returns>
    Task<UserEntity?> GetUserByIdAsync(Guid id);

    /// <summary>
    ///     Получить пользоватя по имени.
    /// </summary>
    /// <param name="username">Имя прользователя.</param>
    /// <returns></returns>
    Task<UserEntity?> GetUserByUsernameAsync(string username);

    /// <summary>
    ///     Получить пользоватя по почте.
    /// </summary>
    /// <param name="email">Почта.</param>
    /// <returns><see cref="UserEntity" />.</returns>
    Task<UserEntity?> GetUserByEmailAsync(string email);

    /// <summary>
    ///     Получить все пользователей.
    /// </summary>
    /// <returns><see cref="UserEntity" />.</returns>
    Task<List<UserEntity>> GetAllUsersAsync();

    /// <summary>
    ///     Создать пользователя.
    /// </summary>
    /// <param name="userEntity"></param>
    /// <returns>Задача.</returns>
    Task CreateUserAsync(UserEntity userEntity);

    /// <summary>
    ///     Обновить пользователя.
    /// </summary>
    /// <param name="userEntity"><see cref="UserEntity" />.</param>
    /// <returns>Задача.</returns>
    Task UpdateUserAsync(UserEntity userEntity);

    /// <summary>
    ///     Существует ли пользователь.
    /// </summary>
    /// <param name="username">Имя пользователя.</param>
    /// <param name="email">Почта.</param>
    /// <returns>Существует или нет.</returns>
    Task<bool> UserExistsAsync(string username, string email);

    /// <summary>
    ///     Получить роли пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список ролей.</returns>
    Task<List<string>> GetUserRolesAsync(Guid userId);

    /// <summary>
    ///     Добавить новую роль пользователю.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="roleNames">Список ролей.</param>
    /// <returns>Задача.</returns>
    Task AssignRolesToUserAsync(Guid userId, IEnumerable<string> roleNames);
}