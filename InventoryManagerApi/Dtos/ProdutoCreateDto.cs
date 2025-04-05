using InventoryManagerApi.Models;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagerApi.Dtos
{
    public class ProdutoCreateDto
    {
        public int? Id { get; set; }

        [Required, MaxLength(255)]
        public string Nome { get; set; }

        [MaxLength(500)]
        public string Descricao { get; set; }

        [Required]
        public int UnidadeCompraId { get; set; }

        [Required]
        public int MenorUnidadeId { get; set; }
        public int Quantidade { get; set; }
        public ICollection<ProdutoUnidadeVendaCreateDto> UnidadesVenda { get; set; } = new List<ProdutoUnidadeVendaCreateDto>();
        public bool Status { get; set; } = true;
    }
}
