using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepositoryLibrary.Migrations {

    public partial class AddColumnNameToChatsTable : Migration {

        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Chats",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Chats");
        }
    }
}