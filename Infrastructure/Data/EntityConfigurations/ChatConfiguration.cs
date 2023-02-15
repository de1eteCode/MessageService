using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.EntityConfigurations;

internal class ChatConfiguration : IEntityTypeConfiguration<Chat> {

    public void Configure(EntityTypeBuilder<Chat> builder) {
        builder.HasKey(e => e.UID);

        builder.Property(e => e.UID).HasDefaultValueSql("uuid_generate_v4()");
        builder.Property(e => e.KickedByUserLogin).HasMaxLength(255);
        builder.Property(e => e.KickedTime).HasPrecision(0);
        builder.Property(e => e.Name).HasMaxLength(255);
    }
}