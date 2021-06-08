using Microsoft.EntityFrameworkCore.Migrations;

namespace FriendOrganizer.DataAccess.Migrations
{
    public partial class RemovedRowVersion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "ProgrammingLanguage");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Meeting");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "FriendPhoneNumber");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Friend");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "ProgrammingLanguage",
                type: "INTEGER",
                rowVersion: true,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Meeting",
                type: "INTEGER",
                rowVersion: true,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "FriendPhoneNumber",
                type: "INTEGER",
                rowVersion: true,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Friend",
                type: "INTEGER",
                rowVersion: true,
                nullable: false,
                defaultValue: 0);
        }
    }
}
