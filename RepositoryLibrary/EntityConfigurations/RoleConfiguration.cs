using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RepositoryLibrary.Models;

namespace RepositoryLibrary.EntityConfigurations;

internal class RoleConfiguration : IEntityTypeConfiguration<Role> {
    public void Configure(EntityTypeBuilder<Role> builder) {
        builder
            .HasKey(e => e.RoleId);

        builder
            .Property(e => e.RoleId)
            .UseIdentityAlwaysColumn(); //

        builder
            .Property(e => e.RoleName)
            .IsRequired();

        builder.HasData(
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
    }
}