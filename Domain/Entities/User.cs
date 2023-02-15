using Domain.Common;

namespace Domain.Entities;

/// <summary>
/// Пользователь в системе
/// </summary>
public class User : BaseEntity {

    public User() {
        UserGroups = new HashSet<UserGroup>();
    }

    /// <summary>
    /// Идентификатор Telegram
    /// </summary>
    public long TelegramId { get; set; }

    /// <summary>
    /// Имя пользователя
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Идентификатор <see cref="Role"/>
    /// </summary>
    public Guid RoleUID { get; set; }

    /// <summary>
    /// Роль пользователя
    /// </summary>
    public virtual Role Role { get; set; } = null!;

    /// <summary>
    /// Группы, в которые включен пользователь
    /// </summary>
    public virtual ICollection<UserGroup> UserGroups { get; set; }
}