using System.ComponentModel.DataAnnotations;

namespace InventoryManagerApi.Dtos
{
    public class UnidadeMedidaDto
    {
        public int? Id { get; set; }
        [Required, MaxLength(25)]
        public string Nome { get; set; }

        [Required, MaxLength(10)]
        public string Sigla { get; set; }

        public bool Status { get; set; } = true;
    }
}
