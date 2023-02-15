using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.EntityConfigurations;

internal class RoleConfiguration : IEntityTypeConfiguration<Role> {

    public void Configure(EntityTypeBuilder<Role> builder) {
        builder.HasKey(e => e.UID);

        builder.HasIndex(e => e.AlternativeId, "roles_alternativeid_unique")
            .IsUnique();

        builder.Property(e => e.AlternativeId).ValueGeneratedOnAdd();

        builder.Property(e => e.UID).HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(e => e.Name).HasMaxLength(120);

        builder.HasData(
            new Role() {
                UID = new Guid("7ADF9DA4-C9B4-4C6D-A75D-2666475BA18E"),
                AlternativeId = 1,
                Name = "Системный администратор"
            },
            new Role() {
                UID = new Guid("422932F4-031C-4ECA-BA53-086287704B60"),
                AlternativeId = 2,
                Name = "Администратор"
            },
            new Role() {
                UID = new Guid("508C2BF9-C65E-443C-9D0E-D53A1B745C53"),
                AlternativeId = 3,
                Name = "Пользователь"
            });
    }
}