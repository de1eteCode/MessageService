using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MessageService.Migrations
{
    public partial class addchatmetadata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsJoined",
                table: "Chats",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "KickedByUserLogin",
                table: "Chats",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "KickedTime",
                table: "Chats",
                type: "timestamp with time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsJoined",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "KickedByUserLogin",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "KickedTime",
                table: "Chats");
        }
    }
}
