using Microsoft.EntityFrameworkCore.Migrations;

namespace FriendOrganizer.DataAccess.Migrations
{
    public partial class AddedFriendPhoneNumbers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FriendPhoneNumber",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Number = table.Column<string>(type: "TEXT", nullable: false),
                    FriendId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendPhoneNumber", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FriendPhoneNumber_Friend_FriendId",
                        column: x => x.FriendId,
                        principalTable: "Friend",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "FriendPhoneNumber",
                columns: new[] { "Id", "FriendId", "Number" },
                values: new object[] { 1, 1, "+49 12345678" });

            migrationBuilder.InsertData(
                table: "ProgrammingLanguage",
                columns: new[] { "Id", "Name" },
                values: new object[] { 5, "F#" });

            migrationBuilder.CreateIndex(
                name: "IX_FriendPhoneNumber_FriendId",
                table: "FriendPhoneNumber",
                column: "FriendId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FriendPhoneNumber");

            migrationBuilder.DeleteData(
                table: "ProgrammingLanguage",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
