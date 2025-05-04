using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagerApi.Migrations
{
    /// <inheritdoc />
    public partial class AddMovimentacaoEstoque : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClienteFornecedorId",
                table: "Pedidos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Observacao",
                table: "Pedidos",
                type: "TEXT",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Pedidos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProdutoUnidadeVendaId",
                table: "ItensPedido",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MovimentacoesEstoque",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProdutoId = table.Column<int>(type: "INTEGER", nullable: false),
                    PedidoId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantidade = table.Column<int>(type: "INTEGER", nullable: false),
                    DataMovimentacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Tipo = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimentacoesEstoque", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovimentacoesEstoque_Pedidos_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "Pedidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovimentacoesEstoque_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_ClienteFornecedorId",
                table: "Pedidos",
                column: "ClienteFornecedorId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensPedido_ProdutoUnidadeVendaId",
                table: "ItensPedido",
                column: "ProdutoUnidadeVendaId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimentacoesEstoque_PedidoId",
                table: "MovimentacoesEstoque",
                column: "PedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimentacoesEstoque_ProdutoId",
                table: "MovimentacoesEstoque",
                column: "ProdutoId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItensPedido_ProdutosUnidadeVenda_ProdutoUnidadeVendaId",
                table: "ItensPedido",
                column: "ProdutoUnidadeVendaId",
                principalTable: "ProdutosUnidadeVenda",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_ClientesFornecedores_ClienteFornecedorId",
                table: "Pedidos",
                column: "ClienteFornecedorId",
                principalTable: "ClientesFornecedores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItensPedido_ProdutosUnidadeVenda_ProdutoUnidadeVendaId",
                table: "ItensPedido");

            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_ClientesFornecedores_ClienteFornecedorId",
                table: "Pedidos");

            migrationBuilder.DropTable(
                name: "MovimentacoesEstoque");

            migrationBuilder.DropIndex(
                name: "IX_Pedidos_ClienteFornecedorId",
                table: "Pedidos");

            migrationBuilder.DropIndex(
                name: "IX_ItensPedido_ProdutoUnidadeVendaId",
                table: "ItensPedido");

            migrationBuilder.DropColumn(
                name: "ClienteFornecedorId",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "Observacao",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "ProdutoUnidadeVendaId",
                table: "ItensPedido");
        }
    }
}
