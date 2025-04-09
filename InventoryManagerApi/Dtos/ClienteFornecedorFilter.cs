using InventoryManagerApi.Enums;

namespace InventoryManagerApi.Dtos
{
    public class ClienteFornecedorFilter
    {
        public string? Nome { get; set; }
        public string? CpfCnpj { get; set; }
        public ETipoClienteFornecedor? Tipo { get; set; }
        public bool? Status { get; set; }
        public string? SearchType { get; set; }
    }
}
