using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Application.Common.Interfaces;

public interface IDataContext : IDisposable {
    public DbSet<Chat> Chats { get; set; }
    public DbSet<ChatGroup> ChatGroups { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserGroup> UserGroups { get; set; }

    public EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

    public int SaveChanges();

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    public Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default);
}