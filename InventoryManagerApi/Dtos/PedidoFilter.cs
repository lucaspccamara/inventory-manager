using InventoryManagerApi.Enums;

namespace InventoryManagerApi.Dtos
{
    public class PedidoFilter
    {
        public int? Id { get; set; }
        public int? ClienteFornecedorId { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public EStatusPedido? Status { get; set; }
        public ETipoMovimentacao Tipo { get; set; }
    }
}
