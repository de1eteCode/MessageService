namespace DataLibrary.Models;

public class UserGroup : BaseModelEntity {
    public Guid GroupUID { get; set; }
    public Guid UserUID { get; set; }

    public virtual Group Group { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}