using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagerApi.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexToClienteFornecedorNome : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ClientesFornecedores_Nome",
                table: "ClientesFornecedores",
                column: "Nome",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ClientesFornecedores_Nome",
                table: "ClientesFornecedores");
        }
    }
}
