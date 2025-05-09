using InventoryManagerApi.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagerApi.Dtos
{
    public class PedidoPdfDto
    {
        public int Id { get; set; }
        public EStatusPedido Status { get; set; }
        public string Observacao { get; set; }
        public DateTime Data { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }
        public ClienteFornecedorDto ClienteFornecedor { get; set; }
        public ICollection<ItemPedidoDto> Itens { get; set; } = new List<ItemPedidoDto>();
    }
}
