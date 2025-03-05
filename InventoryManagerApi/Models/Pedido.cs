namespace InventoryManagerApi.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public DateTime Data { get; set; } = DateTime.UtcNow;
        public List<ItemPedido> Itens { get; set; } = new();
    }
}
