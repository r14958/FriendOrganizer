using Microsoft.EntityFrameworkCore.Migrations;

namespace FriendOrganizer.DataAccess.Migrations
{
    public partial class TryAddingRowVersionAgain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "ProgrammingLanguage",
                type: "INTEGER",
                rowVersion: true,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(
                @"CREATE TRIGGER UpdateProgrammingLanguageVersion
                AFTER UPDATE ON ProgrammingLanguage
                BEGIN
                    UPDATE ProgrammingLanguage
                    SET Version = Version + 1
                    WHERE rowid = NEW.rowid;
                END;");

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Meeting",
                type: "INTEGER",
                rowVersion: true,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(
                @"CREATE TRIGGER UpdateMeetingVersion
                AFTER UPDATE ON Meeting
                BEGIN
                    UPDATE Meeting
                    SET Version = Version + 1
                    WHERE rowid = NEW.rowid;
                END;");

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "FriendPhoneNumber",
                type: "INTEGER",
                rowVersion: true,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(
                @"CREATE TRIGGER UpdateFriendPhoneNumberVersion
                AFTER UPDATE ON FriendPhoneNumber
                BEGIN
                    UPDATE FriendPhoneNumber
                    SET Version = Version + 1
                    WHERE rowid = NEW.rowid;
                END;");

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Friend",
                type: "INTEGER",
                rowVersion: true,
                nullable: false,
                defaultValue: 0);
            
            migrationBuilder.Sql(
                @"CREATE TRIGGER UpdateFriendVersion
                AFTER UPDATE ON Friend
                BEGIN
                    UPDATE Friend
                    SET Version = Version + 1
                    WHERE rowid = NEW.rowid;
                END;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
