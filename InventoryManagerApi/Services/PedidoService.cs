using InventoryManagerApi.Dtos;
using InventoryManagerApi.Models;
using InventoryManagerApi.Repositories;

namespace InventoryManagerApi.Services
{
    public class PedidoService
    {
        private readonly PedidoRepository _pedidoRepository;
        private readonly ItemPedidoRepository _itemPedidoRepository;

        public PedidoService(PedidoRepository pedidoRepository, ItemPedidoRepository itemPedidoRepository)
        {
            _pedidoRepository = pedidoRepository;
            _itemPedidoRepository = itemPedidoRepository;
        }

        public async Task<PagedResponse<PedidoDtoTable>> ListarPedidosAsync(PagedRequest<PedidoFilter> request)
        {
            return await _pedidoRepository.GetPagedAsync(request);
        }

        public async Task<PedidoDto?> ObterPorIdAsync(int id)
        {
            return await _pedidoRepository.GetPedidoDtoByIdAsync(id);
        }

        public async Task<int> AdicionarAsync(PedidoCreateDto pedidoCreateDto)
        {
            var pedido = new Pedido
            {
                ClienteFornecedorId = pedidoCreateDto.ClienteFornecedorId,
                Observacao = pedidoCreateDto.Observacao,
                Data = pedidoCreateDto.Data,
                Status = pedidoCreateDto.Status,
                Total = pedidoCreateDto.Itens.Sum(item => item.Quantidade * item.PrecoUnitario)
            };

            await _pedidoRepository.AddAsync(pedido);

            foreach (var item in pedidoCreateDto.Itens)
            {
                var itemPedido = new ItemPedido
                {
                    PedidoId = pedido.Id,
                    ProdutoId = item.ProdutoId,
                    ProdutoUnidadeVendaId = item.ProdutoUnidadeVendaId,
                    FatorConversao = item.FatorConversao,
                    PrecoUnitario = item.PrecoUnitario,
                    Quantidade = item.Quantidade
                };

                await _itemPedidoRepository.AddAsync(itemPedido);
            }

            return pedido.Id;
        }

        public async Task AtualizarAsync(Pedido pedido)
        {
            await _pedidoRepository.UpdateAsync(pedido);
        }

        public async Task RemoverAsync(int id)
        {
            await _pedidoRepository.DeleteAsync(id);
        }
    }
}
