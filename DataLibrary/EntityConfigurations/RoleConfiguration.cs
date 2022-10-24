using DataLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLibrary.EntityConfigurations;

internal class RoleConfiguration : IEntityTypeConfiguration<Role> {

    public void Configure(EntityTypeBuilder<Role> builder) {
        builder.HasKey(e => e.UID)
            .HasName("Roles_pkey");

        builder.Property(e => e.UID).HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(e => e.Name).HasMaxLength(120);
    }
}
