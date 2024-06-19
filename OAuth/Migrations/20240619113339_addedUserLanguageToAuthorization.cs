using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OAuth.Migrations
{
    /// <inheritdoc />
    public partial class addedUserLanguageToAuthorization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserLanguage",
                table: "AuthCodeChallenge",
                type: "nvarchar(12)",
                maxLength: 12,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserLanguage",
                table: "AuthCodeChallenge");
        }
    }
}
