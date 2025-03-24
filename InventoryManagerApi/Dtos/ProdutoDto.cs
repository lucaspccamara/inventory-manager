using InventoryManagerApi.Models;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagerApi.Dtos
{
    public class ProdutoDto
    {
        public int? Id { get; set; }

        [Required, MaxLength(255)]
        public string Nome { get; set; }

        public string Descricao { get; set; }

        [Required]
        public int UnidadeCompraId { get; set; }

        [Required]
        public int MenorUnidadeId { get; set; }

        public ICollection<ProdutoUnidadeVendaDto> UnidadesVenda { get; set; } = new List<ProdutoUnidadeVendaDto>();

        public bool Status { get; set; } = true;
    }
}
