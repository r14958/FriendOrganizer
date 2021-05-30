using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FriendOrganizer.DataAccess.Migrations
{
    public partial class DropMeetings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "FriendPhoneNumber",
                keyColumn: "Id",
                keyValue: 1,
                column: "Number",
                value: "+49-5361-9-0");

            migrationBuilder.DropTable(
                name: "FriendMeeting (Dictionary<string, object>)");

            migrationBuilder.DropTable(
                name: "Meeting");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Meeting",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.UpdateData(
                table: "FriendPhoneNumber",
                keyColumn: "Id",
                keyValue: 1,
                column: "Number",
                value: "+49 12345678");
        }
    }
}
