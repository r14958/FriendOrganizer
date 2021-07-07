using Microsoft.EntityFrameworkCore.Migrations;

namespace FriendOrganizer.DataAccess.Migrations
{
    public partial class AddedIsProgrammerToFriendEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Friend",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<string>(
                name: "Address_City",
                table: "Friend",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Address_Id",
                table: "Friend",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_Street",
                table: "Friend",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_StreetNumber",
                table: "Friend",
                type: "TEXT",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeveloper",
                table: "Friend",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Friend",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Address_City", "Address_Id", "Address_Street", "Address_StreetNumber" },
                values: new object[] { "Müllheim", 1, "Elmstreet", "12345" });

            migrationBuilder.UpdateData(
                table: "Friend",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Address_City", "Address_Id", "Address_Street", "Address_StreetNumber" },
                values: new object[] { "Boxford", 2, "King John Drive", "6" });

            migrationBuilder.UpdateData(
                table: "Friend",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Address_City", "Address_Id", "Address_Street", "Address_StreetNumber" },
                values: new object[] { "Tiengen", 3, "Hardstreet", "5" });

            migrationBuilder.UpdateData(
                table: "Friend",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Address_City", "Address_Id", "Address_Street", "Address_StreetNumber" },
                values: new object[] { "Neuenburg", 4, "Rheinweg", "4" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address_City",
                table: "Friend");

            migrationBuilder.DropColumn(
                name: "Address_Id",
                table: "Friend");

            migrationBuilder.DropColumn(
                name: "Address_Street",
                table: "Friend");

            migrationBuilder.DropColumn(
                name: "Address_StreetNumber",
                table: "Friend");

            migrationBuilder.DropColumn(
                name: "IsDeveloper",
                table: "Friend");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Friend",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    City = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    FriendId = table.Column<int>(type: "INTEGER", nullable: false),
                    Street = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    StreetNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
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
    }
}
