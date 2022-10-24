namespace DataLibrary.Models;

public class User : BaseModelEntity {

    public User() {
        UserGroups = new HashSet<UserGroup>();
    }

    public long TelegramId { get; set; }
    public string Name { get; set; } = null!;
    public Guid RoleUID { get; set; }

    public virtual Role Role { get; set; } = null!;
    public virtual ICollection<UserGroup> UserGroups { get; set; }
}