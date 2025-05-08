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

        public async Task AtualizarAsync(PedidoUpdateDto pedidoCreateDto)
        {
            var pedido = new Pedido
            {
                Id = pedidoCreateDto.Id.Value,
                ClienteFornecedorId = pedidoCreateDto.ClienteFornecedorId,
                Observacao = pedidoCreateDto.Observacao,
                Data = pedidoCreateDto.Data,
                Status = pedidoCreateDto.Status,
                Total = pedidoCreateDto.Itens.Sum(item => item.Quantidade * item.PrecoUnitario)
            };

            await _pedidoRepository.UpdateAsync(pedido);

            foreach (var item in pedidoCreateDto.ItensAdicionados)
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

            foreach (var item in pedidoCreateDto.ItensModificados)
            {
                var itemPedido = new ItemPedido
                {
                    Id = item.Id.Value,
                    PedidoId = pedido.Id,
                    ProdutoId = item.ProdutoId,
                    ProdutoUnidadeVendaId = item.ProdutoUnidadeVendaId,
                    FatorConversao = item.FatorConversao,
                    PrecoUnitario = item.PrecoUnitario,
                    Quantidade = item.Quantidade
                };

                await _itemPedidoRepository.UpdateAsync(itemPedido);
            }

            foreach (var itemId in pedidoCreateDto.ItensRemovidos)
            {
                await _itemPedidoRepository.DeleteAsync(itemId);
            }
        }

        public async Task CancelarAsync(int id)
        {
            await _pedidoRepository.CancelarAsync(id);
        }

        public async Task RestaurarAsync(int id)
        {
            await _pedidoRepository.RestaurarAsync(id);
        }
    }
}
