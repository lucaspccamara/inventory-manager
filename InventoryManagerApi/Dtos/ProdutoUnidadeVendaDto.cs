using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagerApi.Dtos
{
    public class ProdutoUnidadeVendaDto
    {
        public int? Id { get; set; }

        [Required]
        public int ProdutoId { get; set; }

        [Required]
        public int UnidadeMedidaId { get; set; }

        [Required]
        public int MenorUnidadeId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Fator { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecoPadrao { get; set; }

        public bool Status { get; set; } = true;
    }
}
