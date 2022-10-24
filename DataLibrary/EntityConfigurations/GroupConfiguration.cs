using DataLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLibrary.EntityConfigurations;

internal class GroupConfiguration : IEntityTypeConfiguration<Group> {

    public void Configure(EntityTypeBuilder<Group> builder) {
        builder.HasKey(e => e.UID)
            .HasName("Groups_pkey");

        builder.HasIndex(e => e.AlternativeId, "groups_alternativeid_unique")
            .IsUnique();

        builder.Property(e => e.UID).HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(e => e.Name).HasMaxLength(255);
    }
}
