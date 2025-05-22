using InventoryManagerApi.Enums;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagerApi.Models
{
    public class ClienteFornecedor
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Nome { get; set; }

        [MaxLength(18)]
        public string CpfCnpj { get; set; }

        [MaxLength(255)]
        public string Email { get; set; }

        [MaxLength(15)]
        public string Telefone { get; set; }

        [MaxLength(15)]
        public string Celular { get; set; }

        [MaxLength(500)]
        public string Endereco { get; set; }

        [Required]
        public ETipoClienteFornecedor Tipo { get; set; }

        [Required]
        public bool Status { get; set; }
    }
}
