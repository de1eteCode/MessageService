namespace Domain.Models;

/// <summary>
/// Описание сущности Telegram чата
/// </summary>
public class Chat : BaseEntity {

    public Chat() {
        ChatGroups = new HashSet<ChatGroup>();
    }

    /// <summary>
    /// Идентификатор чата в Telegram
    /// </summary>
    public long TelegramChatId { get; set; }

    /// <summary>
    /// Наименование чата
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Состоит бот в чате
    /// </summary>
    public bool IsJoined { get; set; }

    /// <summary>
    /// Идентификатор Telegram пользователя, который кикнул бота из чата
    /// </summary>
    public long? KickedByUserId { get; set; }

    /// <summary>
    /// Логин Telegram пользователя, который кикнул бота из чата
    /// </summary>
    public string? KickedByUserLogin { get; set; }

    /// <summary>
    /// Дата кика бота из чата
    /// </summary>
    public DateTime? KickedTime { get; set; }

    /// <summary>
    /// Объединение чата в группы
    /// </summary>
    public virtual ICollection<ChatGroup> ChatGroups { get; set; }
}