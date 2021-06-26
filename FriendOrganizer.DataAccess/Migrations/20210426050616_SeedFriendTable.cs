using Microsoft.EntityFrameworkCore.Migrations;

namespace FriendOrganizer.DataAccess.Migrations
{
    public partial class SeedFriendTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Friend",
                columns: new[] { "Id", "Email", "FirstName", "LastName", "Address" },
                values: new object[] { 1, null, "Thomas", "Huber", null });

            migrationBuilder.InsertData(
                table: "Friend",
                columns: new[] { "Id", "Email", "FirstName", "LastName", "Address" },
                values: new object[] { 2, null, "Jeff", "Klein", null });

            migrationBuilder.InsertData(
                table: "Friend",
                columns: new[] { "Id", "Email", "FirstName", "LastName", "Address" },
                values: new object[] { 3, null, "Andreas", "Boehler", null });

            migrationBuilder.InsertData(
                table: "Friend",
                columns: new[] { "Id", "Email", "FirstName", "LastName", "Address" },
                values: new object[] { 4, null, "Chrissi", "Egin", null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Friend",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Friend",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Friend",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Friend",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
