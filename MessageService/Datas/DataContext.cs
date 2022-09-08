using MessageService.Datas.Models;
using Microsoft.EntityFrameworkCore;

namespace MessageService.Datas;

public class DataContext : DbContext {

    public DataContext(DbContextOptions options) : base(options) {
    }

    public DbSet<Chat>? Chats { get; set; }
    public DbSet<Group>? Groups { get; set; }
    public DbSet<ChatGroup>? ChatGroups { get; set; }
    public DbSet<User>? Users { get; set; }
    public DbSet<Role>? Roles { get; set; }
}