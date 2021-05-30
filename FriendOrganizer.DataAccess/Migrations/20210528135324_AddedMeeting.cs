using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FriendOrganizer.DataAccess.Migrations
{
    public partial class AddedMeeting : Migration
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FriendMeeting (Dictionary<string, object>)");

            migrationBuilder.DropTable(
                name: "Meeting");
        }
    }
}
