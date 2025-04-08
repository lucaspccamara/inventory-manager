using InventoryManagerApi.Enums;

namespace InventoryManagerApi.Dtos
{
    public class ClienteFornecedorDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string CpfCnpj { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string Celular { get; set; }
        public string Endereco { get; set; }
        public ETipoClienteFornecedor Tipo { get; set; }
        public bool Status { get; set; }
    }
}
