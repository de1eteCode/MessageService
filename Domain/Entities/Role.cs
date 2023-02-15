using Domain.Common;

namespace Domain.Entities;

/// <summary>
/// Роль <see cref="User"/> в системе
/// </summary>
public class Role : BaseEntity {

    public Role() {
        Users = new HashSet<User>();
    }

    /// <summary>
    /// Наименование роли
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Альтернативный идентификатор сущности <see cref="Role"/> для отображения пользователям
    /// </summary>
    public int AlternativeId { get; set; }

    /// <summary>
    /// Включенные пользователи в роль
    /// </summary>
    public virtual ICollection<User> Users { get; set; }
}