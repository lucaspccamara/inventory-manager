namespace InventoryManagerApi.Dtos
{
    public class ItemPedidoDto
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public string ProdutoNome { get; set; }
        public int ProdutoUnidadeVendaId { get; set; }
        public string UnidadeMedidaNome { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
    }
}
