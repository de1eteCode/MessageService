namespace MessageService.Datas.Models;

public class User {
    public string? Id { get; set; }
    public string? Name { get; set; }
    public int RoleId { get; set; }
    public Role? Role { get; set; }
}