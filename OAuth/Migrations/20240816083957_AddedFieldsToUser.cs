using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OAuth.Migrations
{
    /// <inheritdoc />
    public partial class AddedFieldsToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "AuthUsers",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AuthUsers",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Patronymic",
                table: "AuthUsers",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AuthUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FirstName", "LastName", "PasswordHash", "Patronymic" },
                values: new object[] { "Jack", "Daniels", "$2a$11$zEvf2CP8Mp76j84h0LSyWejt8V0OS.Nxfa7tw3aSYCl98r5XvV8ty", "Morgan" });

            migrationBuilder.UpdateData(
                table: "AuthUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FirstName", "LastName", "PasswordHash", "Patronymic" },
                values: new object[] { "Admin", "Adminov", "$2a$11$NKHr0CC.cSsOv/9ENg4ROuVktKI6TXUAxA7x/ijPtEJxJnUMftE02", "Adminovych" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "AuthUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AuthUsers");

            migrationBuilder.DropColumn(
                name: "Patronymic",
                table: "AuthUsers");

            migrationBuilder.UpdateData(
                table: "AuthUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$HBws4FbV6Isjf6Q.9joa7.Arl0Zd8sO6JijAF7PHFAPt83AlCG5Oy");

            migrationBuilder.UpdateData(
                table: "AuthUsers",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$0vCcHhtBtqYfg.YymaP7yeXmJpVmLnvL2/1J551YGKmKzvxAhXX7y");
        }
    }
}
