namespace DataLibrary.Models;

public class Group : BaseModelEntity {

    public Group() {
        ChatGroups = new HashSet<ChatGroup>();
        UserGroups = new HashSet<UserGroup>();
    }

    public int AlternativeId { get; set; }
    public string Name { get; set; } = null!;

    public virtual ICollection<ChatGroup> ChatGroups { get; set; }
    public virtual ICollection<UserGroup> UserGroups { get; set; }
}