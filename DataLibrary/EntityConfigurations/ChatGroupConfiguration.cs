using DataLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLibrary.EntityConfigurations;

internal class ChatGroupConfiguration : IEntityTypeConfiguration<ChatGroup> {

    public void Configure(EntityTypeBuilder<ChatGroup> builder) {
        builder.HasKey(e => e.UID)
            .HasName("ChatGroups_pkey");

        builder.Property(e => e.UID).HasDefaultValueSql("uuid_generate_v4()");

        builder.HasOne(d => d.Chat)
            .WithMany(p => p.ChatGroups)
            .HasForeignKey(d => d.ChatUID)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("chatgroups_chatuid_foreign");

        builder.HasOne(d => d.Group)
            .WithMany(p => p.ChatGroups)
            .HasForeignKey(d => d.GroupUID)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("chatgroups_groupuid_foreign");
    }
}