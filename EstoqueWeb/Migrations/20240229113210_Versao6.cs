using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EstoqueWeb.Migrations
{
    /// <inheritdoc />
    public partial class Versao6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Endereco_Cliente_ClienteModelIdUsuario",
                table: "Endereco");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Endereco",
                newName: "IdEndereco");

            migrationBuilder.RenameColumn(
                name: "ClienteModelIdUsuario",
                table: "Endereco",
                newName: "IdUsuario");

            migrationBuilder.AddForeignKey(
                name: "FK_Endereco_Cliente_IdUsuario",
                table: "Endereco",
                column: "IdUsuario",
                principalTable: "Cliente",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Endereco_Cliente_IdUsuario",
                table: "Endereco");

            migrationBuilder.RenameColumn(
                name: "IdEndereco",
                table: "Endereco",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "IdUsuario",
                table: "Endereco",
                newName: "ClienteModelIdUsuario");

            migrationBuilder.AddForeignKey(
                name: "FK_Endereco_Cliente_ClienteModelIdUsuario",
                table: "Endereco",
                column: "ClienteModelIdUsuario",
                principalTable: "Cliente",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
