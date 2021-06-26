using Microsoft.EntityFrameworkCore.Migrations;

namespace FriendOrganizer.DataAccess.Migrations
{
    public partial class AddedAddressEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    City = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Street = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    StreetNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    FriendId = table.Column<int>(type: "INTEGER", nullable: false),
                    Version = table.Column<int>(type: "INTEGER", rowVersion: true, nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Address_Friend_FriendId",
                        column: x => x.FriendId,
                        principalTable: "Friend",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Address",
                columns: new[] { "Id", "City", "FriendId", "Street", "StreetNumber" },
                values: new object[] { 1, "Müllheim", 1, "Elmstreet", "12345" });

            migrationBuilder.InsertData(
                table: "Address",
                columns: new[] { "Id", "City", "FriendId", "Street", "StreetNumber" },
                values: new object[] { 2, "Boxford", 2, "King John Drive", "6" });

            migrationBuilder.InsertData(
                table: "Address",
                columns: new[] { "Id", "City", "FriendId", "Street", "StreetNumber" },
                values: new object[] { 3, "Tiengen", 3, "Hardstreet", "5" });

            migrationBuilder.InsertData(
                table: "Address",
                columns: new[] { "Id", "City", "FriendId", "Street", "StreetNumber" },
                values: new object[] { 4, "Neuenburg", 4, "Rheinweg", "4" });

            migrationBuilder.CreateIndex(
                name: "IX_Address_FriendId",
                table: "Address",
                column: "FriendId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Address");
        }
    }
}
