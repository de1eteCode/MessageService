namespace DataLibrary.Models;

public class Role : BaseModelEntity {

    public Role() {
        Users = new HashSet<User>();
    }

    public string Name { get; set; } = null!;
    public int AlternativeId { get; set; }

    public virtual ICollection<User> Users { get; set; }
}