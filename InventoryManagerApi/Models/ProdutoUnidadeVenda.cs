using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagerApi.Models
{
    public class ProdutoUnidadeVenda
    {
        public int Id { get; set; }

        [Required]
        public int ProdutoId { get; set; }

        [Required]
        public int UnidadeMedidaId { get; set; }

        [Required]
        public int MenorUnidadeId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal FatorConversao { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecoPadrao { get; set; }

        public bool Status { get; set; } = true;

        // Relacionamento com Produto
        public virtual Produto Produto { get; set; }

        // Relacionamento com UnidadeMedida (unidade de compra e menor unidade)
        public virtual UnidadeMedida UnidadeMedida { get; set; }
        public virtual UnidadeMedida MenorUnidade { get; set; }
    }

}
