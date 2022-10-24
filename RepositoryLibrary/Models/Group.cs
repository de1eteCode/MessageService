namespace RepositoryLibrary.Models;

public class Group : BaseModelEntity {
    public int GroupId { get; set; }
    public string? Title { get; set; }
    public virtual ICollection<User>? Users { get; set; }
}