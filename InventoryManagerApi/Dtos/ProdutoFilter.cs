namespace InventoryManagerApi.Dtos
{
    public class ProdutoFilter
    {
        public int? Id { get; set; }
        public string? Nome { get; set; }
        public bool? Status { get; set; }
        public string? SearchType { get; set; }
    }
}
