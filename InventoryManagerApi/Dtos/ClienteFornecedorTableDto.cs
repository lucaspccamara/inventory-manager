using InventoryManagerApi.Enums;

namespace InventoryManagerApi.Dtos
{
    public class ClienteFornecedorTableDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string CpfCnpj { get; set; }
        public string Email { get; set; }
        public ETipoClienteFornecedor Tipo { get; set; }
        public bool Status { get; set; }

        private string _telefone;
        private string _celular;

        public string Telefone
        {
            get => _telefone;
            set
            {
                _telefone = value;
                UpdateContato();
            }
        }

        public string Celular
        {
            get => _celular;
            set
            {
                _celular = value;
                UpdateContato();
            }
        }

        public string Contato { get; private set; }

        private void UpdateContato()
        {
            if (!string.IsNullOrEmpty(Telefone) && !string.IsNullOrEmpty(Celular))
            {
                Contato = $"{Telefone} | {Celular}";
            }
            else if (!string.IsNullOrEmpty(Telefone))
            {
                Contato = Telefone;
            }
            else if (!string.IsNullOrEmpty(Celular))
            {
                Contato = Celular;
            }
            else
            {
                Contato = string.Empty;
            }
        }
    }
}
