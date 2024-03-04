using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class AzurirajTabelu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proizvodi_Kategorija_KategorijaId",
                table: "Proizvodi");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Kategorija",
                table: "Kategorija");

            migrationBuilder.RenameTable(
                name: "Kategorija",
                newName: "Kategorije");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Kategorije",
                table: "Kategorije",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Proizvodi_Kategorije_KategorijaId",
                table: "Proizvodi",
                column: "KategorijaId",
                principalTable: "Kategorije",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proizvodi_Kategorije_KategorijaId",
                table: "Proizvodi");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Kategorije",
                table: "Kategorije");

            migrationBuilder.RenameTable(
                name: "Kategorije",
                newName: "Kategorija");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Kategorija",
                table: "Kategorija",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Proizvodi_Kategorija_KategorijaId",
                table: "Proizvodi",
                column: "KategorijaId",
                principalTable: "Kategorija",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
