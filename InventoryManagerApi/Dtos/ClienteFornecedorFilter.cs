using InventoryManagerApi.Enums;
using System.Text.Json.Serialization;

namespace InventoryManagerApi.Dtos
{
    public class ClienteFornecedorFilter
    {
        public int? Id { get; set; }
        public string? Nome { get; set; }
        public string? CpfCnpj { get; set; }
        public string? Email { get; set; }
        public string? TelefoneCelular { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ETipoClienteFornecedor? Tipo { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ETipoClienteFornecedor[]? ListaTipos { get; set; }
        public bool? Status { get; set; }
        public string? SearchType { get; set; }
    }
}
