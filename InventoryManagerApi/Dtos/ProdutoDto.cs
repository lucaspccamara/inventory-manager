using InventoryManagerApi.Models;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagerApi.Dtos
{
    public class ProdutoDto
    {
        public int? Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public UnidadeMedidaDto? UnidadeCompra { get; set; }
        public UnidadeMedidaDto? MenorUnidade { get; set; }
        public int Quantidade { get; set; }
        public ICollection<ProdutoUnidadeVendaDto> UnidadesVenda { get; set; } = new List<ProdutoUnidadeVendaDto>();
        public bool Status { get; set; } = true;
    }
}
