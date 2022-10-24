using Microsoft.EntityFrameworkCore;
using RepositoryLibrary.EntityConfigurations;
using RepositoryLibrary.Models;

namespace RepositoryLibrary;

public class DataContext : DbContext {

    public DataContext(DbContextOptions options) : base(options) {
    }

    public DbSet<Chat> Chats { get; set; } = default!;
    public DbSet<Group> Groups { get; set; } = default!;
    public DbSet<ChatGroup> ChatGroups { get; set; } = default!;
    public DbSet<User> Users { get; set; } = default!;
    public DbSet<Role> Roles { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder builder) {
        builder.ApplyConfiguration(new ChatConfiguration());
        builder.ApplyConfiguration(new ChatGroupConfiguration());
        builder.ApplyConfiguration(new GroupConfiguration());
        builder.ApplyConfiguration(new RoleConfiguration());
        builder.ApplyConfiguration(new UserConfiguration());
    }
}