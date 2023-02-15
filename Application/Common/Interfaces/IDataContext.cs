using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Application.Common.Interfaces;

public interface IDataContext : IDisposable {
    public DbSet<Chat> Chats { get; }
    public DbSet<ChatGroup> ChatGroups { get; }
    public DbSet<Group> Groups { get; }
    public DbSet<Role> Roles { get; }
    public DbSet<User> Users { get; }
    public DbSet<UserGroup> UserGroups { get; }

    public EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

    public int SaveChanges();

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    public Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default);
}