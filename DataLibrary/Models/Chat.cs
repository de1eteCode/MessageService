namespace DataLibrary.Models;

public class Chat : BaseModelEntity {

    public Chat() {
        ChatGroups = new HashSet<ChatGroup>();
    }

    public long TelegramChatId { get; set; }
    public string Name { get; set; } = null!;
    public bool IsJoined { get; set; }
    public long? KickedByUserId { get; set; }
    public string? KickedByUserLogin { get; set; }
    public DateTime? KickedTime { get; set; }

    public virtual ICollection<ChatGroup> ChatGroups { get; set; }
}