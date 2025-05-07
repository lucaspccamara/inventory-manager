namespace InventoryManagerApi.Dtos
{
    public class ItemPedidoDto
    {
        public int Id { get; set; }
        public int PedidoId { get; set; }
        public int ProdutoId { get; set; }
        public string Nome { get; set; }
        public int Quantidade { get; set; }
        public IEnumerable<ProdutoUnidadeVendaDto> UnidadesVenda { get; set; }
        public UnidadeMedidaDto UnidadeVendaSelecionada { get; set; }
        public decimal PrecoUnitario { get; set; }
    }
}
