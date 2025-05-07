using InventoryManagerApi.Enums;

namespace InventoryManagerApi.Dtos
{
    public class PedidoDto
    {
        public int Id { get; set; }
        public ClienteFornecedorSelectDto ClienteFornecedor { get; set; }
        public DateTime Data { get; set; }
        public EStatusPedido Status { get; set; }
        public string Observacao { get; set; }
        public List<ItemPedidoDto> Itens { get; set; } = new();
    }
}
