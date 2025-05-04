using InventoryManagerApi.Enums;

namespace InventoryManagerApi.Dtos
{
    public class PedidoDto
    {
        public int Id { get; set; }
        public int ClienteFornecedorId { get; set; }
        public string ClienteFornecedorNome { get; set; }
        public DateTime Data { get; set; }
        public EStatusPedido Status { get; set; }
        public string Observacao { get; set; }
        public decimal Total { get; set; }
        public List<ItemPedidoDto> Itens { get; set; } = new();
    }
}
