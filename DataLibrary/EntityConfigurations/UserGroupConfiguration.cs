using DataLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLibrary.EntityConfigurations;

internal class UserGroupConfiguration : IEntityTypeConfiguration<UserGroup> {

    public void Configure(EntityTypeBuilder<UserGroup> builder) {
        builder.HasKey(e => e.UID)
            .HasName("UserGroups_pkey");

        builder.Property(e => e.UID).HasDefaultValueSql("uuid_generate_v4()");

        builder.HasOne(d => d.Group)
            .WithMany(p => p.UserGroups)
            .HasForeignKey(d => d.GroupUID)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("usergroups_groupuid_foreign");

        builder.HasOne(d => d.User)
            .WithMany(p => p.UserGroups)
            .HasForeignKey(d => d.UserUID)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("usergroups_useruid_foreign");
    }
}