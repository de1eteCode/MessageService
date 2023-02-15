using Domain.Common;

namespace Domain.Entities;

/// <summary>
/// Объединение сущностей <see cref="Chat"/> и <see cref="Group"/>
/// </summary>
public class ChatGroup : BaseEntity {

    /// <summary>
    /// Идентификатор сущности <see cref="Chat"/>
    /// </summary>
    public Guid ChatUID { get; set; }

    /// <summary>
    /// Идентификатор сущности <see cref="Group"/>
    /// </summary>
    public Guid GroupUID { get; set; }

    /// <summary>
    /// Является данная связь расторгнутой
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Чат Telegram
    /// </summary>
    public virtual Chat Chat { get; set; } = null!;

    /// <summary>
    /// Группа
    /// </summary>
    public virtual Group Group { get; set; } = null!;
}