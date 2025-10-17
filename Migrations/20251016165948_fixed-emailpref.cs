using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyGamesWeb.Migrations
{
    /// <inheritdoc />
    public partial class fixedemailpref : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EmailPreference",
                table: "EmailPreference");

            migrationBuilder.RenameTable(
                name: "EmailPreference",
                newName: "EmailPreferences");

            migrationBuilder.RenameIndex(
                name: "IX_EmailPreference_UserId",
                table: "EmailPreferences",
                newName: "IX_EmailPreferences_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmailPreferences",
                table: "EmailPreferences",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EmailPreferences",
                table: "EmailPreferences");

            migrationBuilder.RenameTable(
                name: "EmailPreferences",
                newName: "EmailPreference");

            migrationBuilder.RenameIndex(
                name: "IX_EmailPreferences_UserId",
                table: "EmailPreference",
                newName: "IX_EmailPreference_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmailPreference",
                table: "EmailPreference",
                column: "Id");
        }
    }
}
