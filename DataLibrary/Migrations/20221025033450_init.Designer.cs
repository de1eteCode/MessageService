﻿// <auto-generated />
using System;
using DataLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataLibrary.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20221025033450_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "uuid-ossp");
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("DataLibrary.Models.Chat", b =>
                {
                    b.Property<Guid>("UID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<bool>("IsJoined")
                        .HasColumnType("boolean");

                    b.Property<long?>("KickedByUserId")
                        .HasColumnType("bigint");

                    b.Property<string>("KickedByUserLogin")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime?>("KickedTime")
                        .HasPrecision(0)
                        .HasColumnType("timestamp(0) with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<long>("TelegramChatId")
                        .HasColumnType("bigint");

                    b.HasKey("UID");

                    b.ToTable("Chats");
                });

            modelBuilder.Entity("DataLibrary.Models.ChatGroup", b =>
                {
                    b.Property<Guid>("UID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<Guid>("ChatUID")
                        .HasColumnType("uuid");

                    b.Property<Guid>("GroupUID")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.HasKey("UID");

                    b.HasIndex("ChatUID");

                    b.HasIndex("GroupUID");

                    b.ToTable("ChatGroups");
                });

            modelBuilder.Entity("DataLibrary.Models.Group", b =>
                {
                    b.Property<Guid>("UID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<int>("AlternativeId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.HasKey("UID");

                    b.HasIndex(new[] { "AlternativeId" }, "groups_alternativeid_unique")
                        .IsUnique();

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("DataLibrary.Models.Role", b =>
                {
                    b.Property<Guid>("UID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<int>("AlternativeId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(120)
                        .HasColumnType("character varying(120)");

                    b.HasKey("UID");

                    b.HasIndex(new[] { "AlternativeId" }, "roles_alternativeid_unique")
                        .IsUnique();

                    b.ToTable("Roles");

                    b.HasData(
                        new
                        {
                            UID = new Guid("7adf9da4-c9b4-4c6d-a75d-2666475ba18e"),
                            AlternativeId = 1,
                            Name = "Системный администратор"
                        },
                        new
                        {
                            UID = new Guid("422932f4-031c-4eca-ba53-086287704b60"),
                            AlternativeId = 2,
                            Name = "Администратор"
                        },
                        new
                        {
                            UID = new Guid("508c2bf9-c65e-443c-9d0e-d53a1b745c53"),
                            AlternativeId = 3,
                            Name = "Пользователь"
                        });
                });

            modelBuilder.Entity("DataLibrary.Models.User", b =>
                {
                    b.Property<Guid>("UID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<Guid>("RoleUID")
                        .HasColumnType("uuid");

                    b.Property<long>("TelegramId")
                        .HasColumnType("bigint");

                    b.HasKey("UID");

                    b.HasIndex("RoleUID");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("DataLibrary.Models.UserGroup", b =>
                {
                    b.Property<Guid>("UID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<Guid>("GroupUID")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserUID")
                        .HasColumnType("uuid");

                    b.HasKey("UID");

                    b.HasIndex("GroupUID");

                    b.HasIndex("UserUID");

                    b.ToTable("UserGroups");
                });

            modelBuilder.Entity("DataLibrary.Models.ChatGroup", b =>
                {
                    b.HasOne("DataLibrary.Models.Chat", "Chat")
                        .WithMany("ChatGroups")
                        .HasForeignKey("ChatUID")
                        .IsRequired();

                    b.HasOne("DataLibrary.Models.Group", "Group")
                        .WithMany("ChatGroups")
                        .HasForeignKey("GroupUID")
                        .IsRequired();

                    b.Navigation("Chat");

                    b.Navigation("Group");
                });

            modelBuilder.Entity("DataLibrary.Models.User", b =>
                {
                    b.HasOne("DataLibrary.Models.Role", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleUID")
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("DataLibrary.Models.UserGroup", b =>
                {
                    b.HasOne("DataLibrary.Models.Group", "Group")
                        .WithMany("UserGroups")
                        .HasForeignKey("GroupUID")
                        .IsRequired();

                    b.HasOne("DataLibrary.Models.User", "User")
                        .WithMany("UserGroups")
                        .HasForeignKey("UserUID")
                        .IsRequired();

                    b.Navigation("Group");

                    b.Navigation("User");
                });

            modelBuilder.Entity("DataLibrary.Models.Chat", b =>
                {
                    b.Navigation("ChatGroups");
                });

            modelBuilder.Entity("DataLibrary.Models.Group", b =>
                {
                    b.Navigation("ChatGroups");

                    b.Navigation("UserGroups");
                });

            modelBuilder.Entity("DataLibrary.Models.Role", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("DataLibrary.Models.User", b =>
                {
                    b.Navigation("UserGroups");
                });
#pragma warning restore 612, 618
        }
    }
}
