using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RepositoryLibrary.Models;

namespace RepositoryLibrary.EntityConfigurations;

internal class ChatGroupConfiguration : IEntityTypeConfiguration<ChatGroup> {
    public void Configure(EntityTypeBuilder<ChatGroup> builder) {
        builder
            .HasKey(e => e.Id);

        builder
            .Property(e => e.Id)
            .UseIdentityAlwaysColumn(); //

        builder
            .HasOne(e => e.Group)
            .WithMany()
            .HasForeignKey(e => e.GroupId)
            .IsRequired();

        builder
            .HasOne(e => e.Chat)
            .WithMany()
            .HasForeignKey(e => e.ChatId)
            .IsRequired();
    }
}