using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagerApi.Dtos
{
    public class ItemPedidoCreateDto
    {
        public int? Id { get; set; }

        public int? PedidoId { get; set; }

        [Required]
        public int ProdutoId { get; set; }

        [Required]
        public int ProdutoUnidadeVendaId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal FatorConversao { get; set; }

        [Required]
        public int Quantidade { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecoUnitario { get; set; }
    }
}
