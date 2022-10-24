using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RepositoryLibrary.Models;

namespace RepositoryLibrary.EntityConfigurations;

internal class UserConfiguration : IEntityTypeConfiguration<User> {

    public void Configure(EntityTypeBuilder<User> builder) {
        builder
            .HasKey(e => e.Id);

        builder
            .Property(e => e.Name)
            .IsRequired();

        builder
            .HasOne(e => e.Role)
            .WithMany()
            .HasForeignKey(e => e.RoleId);
    }
}