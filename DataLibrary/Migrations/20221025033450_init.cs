using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLibrary.Migrations {

    public partial class init : Migration {

        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "Chats",
                columns: table => new {
                    UID = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "public.uuid_generate_v4()"),
                    TelegramChatId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    IsJoined = table.Column<bool>(type: "boolean", nullable: false),
                    KickedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    KickedByUserLogin = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    KickedTime = table.Column<DateTime>(type: "timestamp(0) with time zone", precision: 0, nullable: true)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Chats", x => x.UID);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new {
                    UID = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "public.uuid_generate_v4()"),
                    AlternativeId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Groups", x => x.UID);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new {
                    UID = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "public.uuid_generate_v4()"),
                    Name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    AlternativeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Roles", x => x.UID);
                });

            migrationBuilder.CreateTable(
                name: "ChatGroups",
                columns: table => new {
                    UID = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "public.uuid_generate_v4()"),
                    ChatUID = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupUID = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_ChatGroups", x => x.UID);
                    table.ForeignKey(
                        name: "FK_ChatGroups_Chats_ChatUID",
                        column: x => x.ChatUID,
                        principalTable: "Chats",
                        principalColumn: "UID");
                    table.ForeignKey(
                        name: "FK_ChatGroups_Groups_GroupUID",
                        column: x => x.GroupUID,
                        principalTable: "Groups",
                        principalColumn: "UID");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new {
                    UID = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "public.uuid_generate_v4()"),
                    TelegramId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    RoleUID = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Users", x => x.UID);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleUID",
                        column: x => x.RoleUID,
                        principalTable: "Roles",
                        principalColumn: "UID");
                });

            migrationBuilder.CreateTable(
                name: "UserGroups",
                columns: table => new {
                    UID = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "public.uuid_generate_v4()"),
                    GroupUID = table.Column<Guid>(type: "uuid", nullable: false),
                    UserUID = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_UserGroups", x => x.UID);
                    table.ForeignKey(
                        name: "FK_UserGroups_Groups_GroupUID",
                        column: x => x.GroupUID,
                        principalTable: "Groups",
                        principalColumn: "UID");
                    table.ForeignKey(
                        name: "FK_UserGroups_Users_UserUID",
                        column: x => x.UserUID,
                        principalTable: "Users",
                        principalColumn: "UID");
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "UID", "AlternativeId", "Name" },
                values: new object[,]
                {
                    { new Guid("422932f4-031c-4eca-ba53-086287704b60"), 2, "Администратор" },
                    { new Guid("508c2bf9-c65e-443c-9d0e-d53a1b745c53"), 3, "Пользователь" },
                    { new Guid("7adf9da4-c9b4-4c6d-a75d-2666475ba18e"), 1, "Системный администратор" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatGroups_ChatUID",
                table: "ChatGroups",
                column: "ChatUID");

            migrationBuilder.CreateIndex(
                name: "IX_ChatGroups_GroupUID",
                table: "ChatGroups",
                column: "GroupUID");

            migrationBuilder.CreateIndex(
                name: "groups_alternativeid_unique",
                table: "Groups",
                column: "AlternativeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "roles_alternativeid_unique",
                table: "Roles",
                column: "AlternativeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserGroups_GroupUID",
                table: "UserGroups",
                column: "GroupUID");

            migrationBuilder.CreateIndex(
                name: "IX_UserGroups_UserUID",
                table: "UserGroups",
                column: "UserUID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleUID",
                table: "Users",
                column: "RoleUID");
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropTable(
                name: "ChatGroups");

            migrationBuilder.DropTable(
                name: "UserGroups");

            migrationBuilder.DropTable(
                name: "Chats");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}