namespace Application.Common.Interfaces;

/// <summary>
/// Сервис по идентификации пользователей
/// </summary>
public interface IIdentityService {

    Task<bool> AuthorizeAsync(Guid userId);

    Task<bool> IsInRoleAsync(Guid userId, string roleName);

    Task<string> GetUserNameAsync(Guid userId);

    Task<object> CreateUserAsync();
}