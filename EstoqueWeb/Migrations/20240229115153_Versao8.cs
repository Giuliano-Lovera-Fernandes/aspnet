using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EstoqueWeb.Migrations
{
    /// <inheritdoc />
    public partial class Versao8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Cliente");

            migrationBuilder.DropColumn(
                name: "Nome",
                table: "Cliente");

            migrationBuilder.DropColumn(
                name: "Senha",
                table: "Cliente");

            migrationBuilder.AlterColumn<int>(
                name: "Estoque",
                table: "Produto",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<bool>(
                name: "Selecionado",
                table: "Endereco",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Pedido",
                columns: table => new
                {
                    IdPedido = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DataPedido = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DataEntrega = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ValorTotal = table.Column<double>(type: "REAL", nullable: true),
                    IdCliente = table.Column<int>(type: "INTEGER", nullable: false),
                    EnderecoEntregaLogradouro = table.Column<string>(name: "EnderecoEntrega_Logradouro", type: "TEXT", nullable: false),
                    EnderecoEntregaNumero = table.Column<string>(name: "EnderecoEntrega_Numero", type: "TEXT", nullable: false),
                    EnderecoEntregaComplemento = table.Column<string>(name: "EnderecoEntrega_Complemento", type: "TEXT", nullable: false),
                    EnderecoEntregaBairro = table.Column<string>(name: "EnderecoEntrega_Bairro", type: "TEXT", nullable: false),
                    EnderecoEntregaCidade = table.Column<string>(name: "EnderecoEntrega_Cidade", type: "TEXT", nullable: false),
                    EnderecoEntregaEstado = table.Column<string>(name: "EnderecoEntrega_Estado", type: "char(2)", nullable: false),
                    EnderecoEntregaCep = table.Column<string>(name: "EnderecoEntrega_Cep", type: "char(9)", nullable: false),
                    EnderecoEntregaReferencia = table.Column<string>(name: "EnderecoEntrega_Referencia", type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pedido", x => x.IdPedido);
                    table.ForeignKey(
                        name: "FK_Pedido_Cliente_IdCliente",
                        column: x => x.IdCliente,
                        principalTable: "Cliente",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Senha = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "TEXT", nullable: true, defaultValueSql: "datetime('now', 'localtime', 'start of day')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.IdUsuario);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_IdCliente",
                table: "Pedido",
                column: "IdCliente");

            migrationBuilder.AddForeignKey(
                name: "FK_Cliente_Usuario_IdUsuario",
                table: "Cliente",
                column: "IdUsuario",
                principalTable: "Usuario",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cliente_Usuario_IdUsuario",
                table: "Cliente");

            migrationBuilder.DropTable(
                name: "Pedido");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropColumn(
                name: "Selecionado",
                table: "Endereco");

            migrationBuilder.AlterColumn<int>(
                name: "Estoque",
                table: "Produto",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldDefaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Cliente",
                type: "TEXT",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Nome",
                table: "Cliente",
                type: "TEXT",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Senha",
                table: "Cliente",
                type: "TEXT",
                maxLength: 128,
                nullable: false,
                defaultValue: "");
        }
    }
}
