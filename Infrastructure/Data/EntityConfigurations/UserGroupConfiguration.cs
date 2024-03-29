﻿using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.EntityConfigurations;

internal class UserGroupConfiguration : IEntityTypeConfiguration<UserGroup> {

    public void Configure(EntityTypeBuilder<UserGroup> builder) {
        builder.HasKey(e => e.UID);

        builder.Property(e => e.UID).HasDefaultValueSql("uuid_generate_v4()");

        builder.HasOne(d => d.Group)
            .WithMany(p => p.UserGroups)
            .HasForeignKey(d => d.GroupUID)
            .OnDelete(DeleteBehavior.ClientSetNull);

        builder.HasOne(d => d.User)
            .WithMany(p => p.UserGroups)
            .HasForeignKey(d => d.UserUID)
            .OnDelete(DeleteBehavior.ClientSetNull);
    }
}