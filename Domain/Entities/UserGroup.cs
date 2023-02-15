using Domain.Common;

namespace Domain.Entities;

/// <summary>
/// Объединение сущностей <see cref="User"/> и <see cref="Group"/>
/// </summary>
public class UserGroup : BaseEntity {

    /// <summary>
    /// Идентификатор <see cref="Group"/>
    /// </summary>
    public Guid GroupUID { get; set; }

    /// <summary>
    /// Идентификатор <see cref="User"/>
    /// </summary>
    public Guid UserUID { get; set; }

    /// <summary>
    /// Группа
    /// </summary>
    public virtual Group Group { get; set; } = null!;

    /// <summary>
    /// Пользователь
    /// </summary>
    public virtual User User { get; set; } = null!;
}