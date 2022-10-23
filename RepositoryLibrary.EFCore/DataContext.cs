using Microsoft.EntityFrameworkCore;
using RepositoryLibrary.Models;

namespace RepositoryLibrary.EFCore;
public class DataContext : DbContext {
    public DataContext(DbContextOptions options) : base(options) {
    }

    public DbSet<Chat> Chats { get; set; } = default!;
    public DbSet<Group> Groups { get; set; } = default!;
    public DbSet<ChatGroup> ChatGroups { get; set; } = default!;
    public DbSet<User> Users { get; set; } = default!;
    public DbSet<Role> Roles { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Group>()
            .HasMany(p => p.Users)
            .WithMany(p => p.Groups)
            .UsingEntity(j => j.ToTable("GroupUser"));

        #region Chat configure

        modelBuilder
            .Entity<Chat>()
            .HasKey(e => e.ChatId);

        modelBuilder
            .Entity<Chat>()
            .Property(e => e.Name)
            .IsRequired();

        modelBuilder
            .Entity<Chat>()
            .Property(e => e.KickedTime)
            .IsRequired(false);

        modelBuilder
            .Entity<Chat>()
            .Property(e => e.KickedByUserLogin)
            .IsRequired(false);

        #endregion Chat configure

        #region Chat group configure

        modelBuilder
            .Entity<ChatGroup>()
            .HasKey(e => e.Id);

        modelBuilder
            .Entity<ChatGroup>()
            .Property(e => e.Id)
            .UseIdentityAlwaysColumn(); //

        modelBuilder
            .Entity<ChatGroup>()
            .HasOne(e => e.Group)
            .WithMany()
            .HasForeignKey(e => e.GroupId)
            .IsRequired();

        modelBuilder
            .Entity<ChatGroup>()
            .HasOne(e => e.Chat)
            .WithMany()
            .HasForeignKey(e => e.ChatId)
            .IsRequired();

        #endregion Chat group configure

        #region Group configure

        modelBuilder
            .Entity<Group>()
            .HasKey(e => e.GroupId);

        modelBuilder
            .Entity<Group>()
            .Property(e => e.GroupId)
            .UseIdentityAlwaysColumn(); //

        modelBuilder
            .Entity<Group>()
            .Property(e => e.Title)
            .IsRequired();

        #endregion Group configure

        #region Role configure

        modelBuilder
            .Entity<Role>()
            .HasKey(e => e.RoleId);

        modelBuilder
            .Entity<Role>()
            .Property(e => e.RoleId)
            .UseIdentityAlwaysColumn(); //

        modelBuilder
            .Entity<Role>()
            .Property(e => e.RoleName)
            .IsRequired();

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

        #endregion Role configure

        #region User configure

        modelBuilder
            .Entity<User>()
            .HasKey(e => e.Id);

        modelBuilder
            .Entity<User>()
            .Property(e => e.Name)
            .IsRequired();

        modelBuilder
            .Entity<User>()
            .HasOne(e => e.Role)
            .WithMany()
            .HasForeignKey(e => e.RoleId);

        #endregion User configure
    }
}
