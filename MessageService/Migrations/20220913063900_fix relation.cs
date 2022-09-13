using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MessageService.Migrations
{
    public partial class fixrelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ChatGroups_ChatId",
                table: "ChatGroups");

            migrationBuilder.DropIndex(
                name: "IX_ChatGroups_GroupId",
                table: "ChatGroups");

            migrationBuilder.CreateIndex(
                name: "IX_ChatGroups_ChatId",
                table: "ChatGroups",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatGroups_GroupId",
                table: "ChatGroups",
                column: "GroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ChatGroups_ChatId",
                table: "ChatGroups");

            migrationBuilder.DropIndex(
                name: "IX_ChatGroups_GroupId",
                table: "ChatGroups");

            migrationBuilder.CreateIndex(
                name: "IX_ChatGroups_ChatId",
                table: "ChatGroups",
                column: "ChatId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChatGroups_GroupId",
                table: "ChatGroups",
                column: "GroupId",
                unique: true);
        }
    }
}
