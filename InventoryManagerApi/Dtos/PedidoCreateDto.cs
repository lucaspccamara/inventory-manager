using InventoryManagerApi.Enums;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagerApi.Dtos
{
    public class PedidoCreateDto
    {
        public int? Id { get; set; }
        public int ClienteFornecedorId { get; set; }
        public EStatusPedido Status { get; set; }

        [MaxLength(500)]
        public string Observacao { get; set; }
        public DateTime Data { get; set; }
        public virtual ICollection<ItemPedidoCreateDto> Itens { get; set; } = new List<ItemPedidoCreateDto>();
    }
}
