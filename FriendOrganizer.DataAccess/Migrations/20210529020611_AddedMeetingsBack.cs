using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FriendOrganizer.DataAccess.Migrations
{
    public partial class AddedMeetingsBack : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Meeting",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    DateFrom = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    DateTo = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meeting", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FriendMeeting (Dictionary<string, object>)",
                columns: table => new
                {
                    FriendsId = table.Column<int>(type: "INTEGER", nullable: false),
                    MeetingsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendMeeting (Dictionary<string, object>)", x => new { x.FriendsId, x.MeetingsId });
                    table.ForeignKey(
                        name: "FK_FriendMeeting (Dictionary<string, object>)_Friend_FriendsId",
                        column: x => x.FriendsId,
                        principalTable: "Friend",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FriendMeeting (Dictionary<string, object>)_Meeting_MeetingsId",
                        column: x => x.MeetingsId,
                        principalTable: "Meeting",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FriendMeeting (Dictionary<string, object>)_MeetingsId",
                table: "FriendMeeting (Dictionary<string, object>)",
                column: "MeetingsId");

            migrationBuilder.InsertData(
                table: "Meeting",
                columns: new[] { "Id", "DateFrom", "DateTo", "Title" },
                values: new object[] { 2, new DateTimeOffset(new DateTime(2021, 5, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, -4, 0, 0, 0)), new DateTimeOffset(new DateTime(2021, 5, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, -4, 0, 0, 0)), "Watching Football" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Meeting",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
