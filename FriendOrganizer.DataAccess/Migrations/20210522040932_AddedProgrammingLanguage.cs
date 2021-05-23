using Microsoft.EntityFrameworkCore.Migrations;

namespace FriendOrganizer.DataAccess.Migrations
{
    public partial class AddedProgrammingLanguage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FavoriteLanguageId",
                table: "Friend",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProgrammingLanguage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgrammingLanguage", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ProgrammingLanguage",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "C#" });

            migrationBuilder.InsertData(
                table: "ProgrammingLanguage",
                columns: new[] { "Id", "Name" },
                values: new object[] { 2, "Python" });

            migrationBuilder.InsertData(
                table: "ProgrammingLanguage",
                columns: new[] { "Id", "Name" },
                values: new object[] { 3, "Swift" });

            migrationBuilder.InsertData(
                table: "ProgrammingLanguage",
                columns: new[] { "Id", "Name" },
                values: new object[] { 4, "Java" });

            migrationBuilder.CreateIndex(
                name: "IX_Friend_FavoriteLanguageId",
                table: "Friend",
                column: "FavoriteLanguageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Friend_ProgrammingLanguage_FavoriteLanguageId",
                table: "Friend",
                column: "FavoriteLanguageId",
                principalTable: "ProgrammingLanguage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friend_ProgrammingLanguage_FavoriteLanguageId",
                table: "Friend");

            migrationBuilder.DropTable(
                name: "ProgrammingLanguage");

            migrationBuilder.DropIndex(
                name: "IX_Friend_FavoriteLanguageId",
                table: "Friend");

            migrationBuilder.DropColumn(
                name: "FavoriteLanguageId",
                table: "Friend");
        }
    }
}
