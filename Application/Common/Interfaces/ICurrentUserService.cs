namespace Application.Common.Interfaces;

/// <summary>
/// Сервис получения текущего пользователя в рамках запроса
/// </summary>
public interface ICurrentUserService {
    public Guid? UserId { get; }
}