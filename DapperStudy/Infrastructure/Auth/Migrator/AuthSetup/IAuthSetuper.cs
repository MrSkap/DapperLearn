using DapperStudy.Models.Models;

namespace DapperStudy.Infrastructure.Auth.Migrator.AuthSetup;

public interface IAuthSetuper
{
    /// <summary>
    ///     Задать дефолтные данные для таблиц пользователей и ролей.
    /// </summary>
    /// <remarks>Создает администратора и базовые роли <see cref="KnownRoles" />.</remarks>
    /// <param name="context">Контекст.</param>
    Task SeedDataAsync(AuthDbContext context);
}