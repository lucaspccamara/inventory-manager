using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagerApi.Models
{
    public class Produto
    {
        public int Id { get; set; }

        [Required, MaxLength(255)]
        public string Nome { get; set; }

        public string Descricao { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecoPadrao { get; set; }

        [Required]
        public int UnidadeMedidaCompraId { get; set; }
        public virtual UnidadeMedida UnidadeMedidaCompra { get; set; }

        [Required]
        public int MenorUnidadeId { get; set; }
        public virtual UnidadeMedida MenorUnidade { get; set; }

        public bool Status { get; set; } = true;

        // Relacionamento com ProdutoUnidadeVenda
        public virtual ICollection<ProdutoUnidadeVenda> UnidadesVenda { get; set; } = new List<ProdutoUnidadeVenda>();
    }
}