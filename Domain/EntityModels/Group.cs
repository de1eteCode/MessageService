namespace Domain.Models;

/// <summary>
/// Объединение чатов и пользователей
/// </summary>
public class Group : BaseEntity {

    public Group() {
        ChatGroups = new HashSet<ChatGroup>();
        UserGroups = new HashSet<UserGroup>();
    }

    /// <summary>
    /// Альтернативный идентификатор сущности <see cref="Group"/> для отображения пользователям
    /// </summary>
    public int AlternativeId { get; set; }

    /// <summary>
    /// Наименование группы
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Включенные чаты в группу
    /// </summary>
    public virtual ICollection<ChatGroup> ChatGroups { get; set; }

    /// <summary>
    /// Включенные пользователи в группу
    /// </summary>
    public virtual ICollection<UserGroup> UserGroups { get; set; }
}