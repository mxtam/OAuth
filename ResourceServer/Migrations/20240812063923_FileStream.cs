using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResourceServer.Migrations
{
    /// <inheritdoc />
    public partial class FileStream : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE TABLE EFB.ResourceFiles AS FileTable
                                    WITH (
                                    FILETABLE_DIRECTORY = 'FileStream',
                                    FILETABLE_COLLATE_FILENAME = database_default );");

            migrationBuilder.CreateTable(
                name: "ResourceFileConnection",
                schema : "EFB",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_ResourceFile = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceFileConnection", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Resource_FileConnections_ResourceFile",
                        column: x => x.ID_ResourceFile,
                        principalSchema: "EFB",
                        principalTable: "ResourceFiles",
                        principalColumn: "stream_id");
                },
                comment: "таблиця для зв'язку файлів");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceFileConnection_ID_ResourceFile",
                schema: "EFB",
                table: "ResourceFileConnection",
                column: "ID_ResourceFile");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResourceFileConnection",
                schema: "EFB");

            migrationBuilder.DropTable(
                name: "ResourceFiles", 
                schema: "EFB");
        }
    }
}
