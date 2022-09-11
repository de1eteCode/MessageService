using MessageService.Datas.Models;
using Microsoft.EntityFrameworkCore;

namespace MessageService.Datas;

public class DataContext : DbContext {

    public DataContext(DbContextOptions options) : base(options) {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Role>().HasData(
            new Role() {
                RoleId = 1,
                RoleName = "Системный администратор"
            },
            new Role() {
                RoleId = 2,
                RoleName = "Администратор"
            },
            new Role() {
                RoleId = 3,
                RoleName = "Пользователь"
            });

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Chat> Chats { get; set; } = default!;
    public DbSet<Group> Groups { get; set; } = default!;
    public DbSet<ChatGroup> ChatGroups { get; set; } = default!;
    public DbSet<User> Users { get; set; } = default!;
    public DbSet<Role> Roles { get; set; } = default!;
}