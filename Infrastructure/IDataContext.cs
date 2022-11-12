using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public interface IDataContext {
    public DbSet<Chat> Chats { get; set; }
    public DbSet<ChatGroup> ChatGroups { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserGroup> UserGroups { get; set; }
}