using InventoryManagerApi.Enums;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagerApi.Dtos
{
    public class PedidoUpdateDto
    {
        public int? Id { get; set; }
        public int ClienteFornecedorId { get; set; }
        public EStatusPedido Status { get; set; }

        [MaxLength(500)]
        public string Observacao { get; set; }
        public DateTime Data { get; set; }
        public virtual ICollection<ItemPedidoCreateDto> Itens { get; set; } = new List<ItemPedidoCreateDto>();
        public virtual ICollection<ItemPedidoCreateDto> ItensAdicionados { get; set; } = new List<ItemPedidoCreateDto>();
        public virtual ICollection<ItemPedidoCreateDto> ItensModificados { get; set; } = new List<ItemPedidoCreateDto>();
        public virtual IEnumerable<int> ItensRemovidos { get; set; } = new List<int>();
    }
}
