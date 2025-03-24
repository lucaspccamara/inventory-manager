namespace InventoryManagerApi.Dtos
{
    public class UnidadeMedidaFilter
    {
        public string? Nome { get; set; }
        public string? Sigla { get; set; }
        public bool? Status { get; set; }
        public string? SearchType { get; set; }
    }
}
