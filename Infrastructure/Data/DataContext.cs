using Application.Common.Interfaces;
using Domain.Models;
using Infrastructure.Data.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

internal class DataContext : DbContext, IDataContext {

    public DataContext(DbContextOptions<DataContext> options)
        : base(options) {
    }

    public virtual DbSet<Chat> Chats { get; set; } = null!;
    public virtual DbSet<ChatGroup> ChatGroups { get; set; } = null!;
    public virtual DbSet<Group> Groups { get; set; } = null!;
    public virtual DbSet<Role> Roles { get; set; } = null!;
    public virtual DbSet<User> Users { get; set; } = null!;
    public virtual DbSet<UserGroup> UserGroups { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.HasPostgresExtension("uuid-ossp");
        modelBuilder.ApplyConfiguration(new ChatConfiguration());
        modelBuilder.ApplyConfiguration(new ChatGroupConfiguration());
        modelBuilder.ApplyConfiguration(new GroupConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new UserGroupConfiguration());
    }
}