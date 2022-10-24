namespace DataLibrary.Models;

public class ChatGroup : BaseModelEntity {
    public Guid ChatUID { get; set; }
    public Guid GroupUID { get; set; }
    public bool IsDeleted { get; set; }

    public virtual Chat Chat { get; set; } = null!;
    public virtual Group Group { get; set; } = null!;
}