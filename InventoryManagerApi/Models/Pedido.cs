using InventoryManagerApi.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagerApi.Models
{
    public class Pedido
    {
        public int Id { get; set; }

        [Required]
        public int ClienteFornecedorId { get; set; }

        [Required]
        public EStatusPedido Status { get; set; }

        [MaxLength(500)]
        public string Observacao { get; set; }

        [Required]
        public DateTime Data { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        // Relacionamento com ClienteFornecedor e Itens
        public virtual ClienteFornecedor ClienteFornecedor { get; set; }
        public virtual ICollection<ItemPedido> Itens { get; set; } = new List<ItemPedido>();
    }
}
