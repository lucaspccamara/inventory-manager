using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagerApi.Migrations
{
    /// <inheritdoc />
    public partial class AddMenorUnidadeIdToProdutoUnidadeVenda : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrecoPadrao",
                table: "Produtos");

            migrationBuilder.AddColumn<int>(
                name: "MenorUnidadeId",
                table: "ProdutosUnidadeVenda",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecoPadrao",
                table: "ProdutosUnidadeVenda",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "ProdutosUnidadeVenda",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_ProdutosUnidadeVenda_MenorUnidadeId",
                table: "ProdutosUnidadeVenda",
                column: "MenorUnidadeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProdutosUnidadeVenda_UnidadesMedida_MenorUnidadeId",
                table: "ProdutosUnidadeVenda",
                column: "MenorUnidadeId",
                principalTable: "UnidadesMedida",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProdutosUnidadeVenda_UnidadesMedida_MenorUnidadeId",
                table: "ProdutosUnidadeVenda");

            migrationBuilder.DropIndex(
                name: "IX_ProdutosUnidadeVenda_MenorUnidadeId",
                table: "ProdutosUnidadeVenda");

            migrationBuilder.DropColumn(
                name: "MenorUnidadeId",
                table: "ProdutosUnidadeVenda");

            migrationBuilder.DropColumn(
                name: "PrecoPadrao",
                table: "ProdutosUnidadeVenda");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ProdutosUnidadeVenda");

            migrationBuilder.AddColumn<decimal>(
                name: "PrecoPadrao",
                table: "Produtos",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
