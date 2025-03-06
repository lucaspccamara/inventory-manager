using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagerApi.Models
{
    public class ProdutoUnidadeVenda
    {
        public int Id { get; set; }

        [Required]
        public int ProdutoId { get; set; }
        public virtual Produto Produto { get; set; }

        [Required]
        public int UnidadeMedidaId { get; set; }
        public virtual UnidadeMedida UnidadeMedida { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal FatorConversao { get; set; }
    }

}
