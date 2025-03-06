using System.ComponentModel.DataAnnotations;

namespace InventoryManagerApi.Models
{
    public class UnidadeMedida
    {
        public int Id { get; set; }

        [Required, MaxLength(25)]
        public string Nome { get; set; }

        [Required, MaxLength(10)]
        public string Sigla { get; set; }

        public bool Status { get; set; } = true;

        // Relacionamento com Produto (unidade de compra e menor unidade)
        public virtual ICollection<Produto> ProdutosCompra { get; set; } = new List<Produto>();
        public virtual ICollection<Produto> ProdutosMenorUnidade { get; set; } = new List<Produto>();

        // Relacionamento com ProdutoUnidadeVenda
        public virtual ICollection<ProdutoUnidadeVenda> ProdutosVenda { get; set; } = new List<ProdutoUnidadeVenda>();
    }

}
