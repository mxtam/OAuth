using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OAuth.Migrations
{
    /// <inheritdoc />
    public partial class updateRoleModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthUsers_Roles_RoleId",
                table: "AuthUsers");

            migrationBuilder.AlterColumn<int>(
                name: "RoleId",
                table: "AuthUsers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AuthUsers_Roles_RoleId",
                table: "AuthUsers",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthUsers_Roles_RoleId",
                table: "AuthUsers");

            migrationBuilder.AlterColumn<int>(
                name: "RoleId",
                table: "AuthUsers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_AuthUsers_Roles_RoleId",
                table: "AuthUsers",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id");
        }
    }
}
