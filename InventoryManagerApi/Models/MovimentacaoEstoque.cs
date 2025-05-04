using InventoryManagerApi.Enums;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagerApi.Models
{
    public class MovimentacaoEstoque
    {
        public int Id { get; set; }

        [Required]
        public int ProdutoId { get; set; }

        [Required]
        public int PedidoId { get; set; }

        [Required]
        public int Quantidade { get; set; }

        [Required]
        public DateTime DataMovimentacao { get; set; } = DateTime.UtcNow;

        [Required]
        public ETipoMovimentacao Tipo { get; set; }

        // Relacionamento com Produto e Pedido
        public Produto Produto { get; set; } = null!;
        public Pedido Pedido { get; set; } = null!;
    }
}
