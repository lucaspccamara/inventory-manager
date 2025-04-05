namespace InventoryManagerApi.Dtos
{
    public class ProdutoDtoTable
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public int Quantidade { get; set; }
        public bool Status { get; set; } = true;
        public List<QuantidadePorUnidadeDto> QuantidadesPorUnidade { get; set; } = new();
    }

    public class QuantidadePorUnidadeDto
    {
        public string Unidade { get; set; }
        public int Quantidade { get; set; }
    }
}
