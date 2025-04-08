using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagerApi.Migrations
{
    /// <inheritdoc />
    public partial class AddClienteFornecedor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClientesFornecedores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    CpfCnpj = table.Column<string>(type: "TEXT", maxLength: 18, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Telefone = table.Column<string>(type: "TEXT", maxLength: 15, nullable: false),
                    Celular = table.Column<string>(type: "TEXT", maxLength: 15, nullable: false),
                    Endereco = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Tipo = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientesFornecedores", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientesFornecedores");
        }
    }
}
