using InventoryManagerApi.Enums;

namespace InventoryManagerApi.Dtos
{
    public class PedidoDtoTable
    {
        public int Id { get; set; }
        public string ClienteFornecedorNome { get; set; }
        public DateTime Data { get; set; }
        public EStatusPedido Status { get; set; }
        public decimal Total { get; set; }
    }
}
