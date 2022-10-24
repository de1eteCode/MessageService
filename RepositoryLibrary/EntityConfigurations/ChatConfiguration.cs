using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RepositoryLibrary.Models;

namespace RepositoryLibrary.EntityConfigurations;

internal class ChatConfiguration : IEntityTypeConfiguration<Chat> {

    public void Configure(EntityTypeBuilder<Chat> builder) {
        builder
            .HasKey(e => e.ChatId);

        builder
            .Property(e => e.Name)
            .IsRequired();

        builder
            .Property(e => e.KickedTime)
            .IsRequired(false);

        builder
            .Property(e => e.KickedByUserLogin)
            .IsRequired(false);
    }
}