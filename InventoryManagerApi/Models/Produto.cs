using InventoryManagerApi.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagerApi.Models
{
    public class Produto
    {
        public int Id { get; set; }

        [Required, MaxLength(255)]
        public string Nome { get; set; }

        [MaxLength(500)]
        public string Descricao { get; set; }

        [Required]
        public int UnidadeMedidaCompraId { get; set; }

        [Required]
        public int MenorUnidadeId { get; set; }
        public int Quantidade { get; set; }
        public bool Status { get; set; } = true;

        //Relacionamento com UnidadeMedida (unidade de compra e menor unidade)
        public virtual UnidadeMedida UnidadeMedidaCompra { get; set; }
        public virtual UnidadeMedida MenorUnidade { get; set; }

        // Relacionamento com ProdutoUnidadeVenda
        public virtual ICollection<ProdutoUnidadeVenda> UnidadesVenda { get; set; } = new List<ProdutoUnidadeVenda>();

        public void AtualizarEstoque(int quantidade, ETipoMovimentacao tipo)
        {
            if (tipo == ETipoMovimentacao.Entrada)
            {
                Quantidade += quantidade;
            }
            else if (tipo == ETipoMovimentacao.Saida)
            {
                Quantidade -= quantidade;
            }
        }
    }
}