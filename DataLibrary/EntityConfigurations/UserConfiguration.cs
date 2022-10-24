using DataLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLibrary.EntityConfigurations;

internal class UserConfiguration : IEntityTypeConfiguration<User> {
    public void Configure(EntityTypeBuilder<User> builder) {
        builder.HasKey(e => e.UID)
            .HasName("Users_pkey");

        builder.Property(e => e.UID).HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(e => e.Name).HasMaxLength(255);

        builder.HasOne(d => d.Role)
            .WithMany(p => p.Users)
            .HasForeignKey(d => d.RoleUID)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("users_roleuid_foreign");
    }
}