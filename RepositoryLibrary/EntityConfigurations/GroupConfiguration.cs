using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RepositoryLibrary.Models;

namespace RepositoryLibrary.EntityConfigurations;

internal class GroupConfiguration : IEntityTypeConfiguration<Group> {

    public void Configure(EntityTypeBuilder<Group> builder) {
        builder
            .HasKey(e => e.GroupId);

        builder
            .Property(e => e.GroupId)
            .UseIdentityAlwaysColumn(); //

        builder
            .Property(e => e.Title)
            .IsRequired();

        // many-to-many
        builder
            .HasMany(p => p.Users)
            .WithMany(p => p.Groups)
            .UsingEntity(j => j.ToTable("GroupUser"));
    }
}