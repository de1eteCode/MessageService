using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.EntityConfigurations;

internal class ChatGroupConfiguration : IEntityTypeConfiguration<ChatGroup> {

    public void Configure(EntityTypeBuilder<ChatGroup> builder) {
        builder.HasKey(e => e.UID);

        builder.Property(e => e.UID).HasDefaultValueSql("uuid_generate_v4()");

        builder.HasOne(d => d.Chat)
            .WithMany(p => p.ChatGroups)
            .HasForeignKey(d => d.ChatUID)
            .OnDelete(DeleteBehavior.ClientSetNull);

        builder.HasOne(d => d.Group)
            .WithMany(p => p.ChatGroups)
            .HasForeignKey(d => d.GroupUID)
            .OnDelete(DeleteBehavior.ClientSetNull);
    }
}