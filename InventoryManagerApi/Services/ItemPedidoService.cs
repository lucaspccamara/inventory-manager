using InventoryManagerApi.Models;
using InventoryManagerApi.Repositories;

namespace InventoryManagerApi.Services
{
    public class ItemPedidoService
    {
        private readonly ItemPedidoRepository _itemPedidoRepository;

        public ItemPedidoService(ItemPedidoRepository itemPedidoRepository)
        {
            _itemPedidoRepository = itemPedidoRepository;
        }

        public async Task<ItemPedido?> ObterPorIdAsync(int id)
        {
            return await _itemPedidoRepository.GetByIdAsync(id);
        }

        public async Task<int> AdicionarAsync(ItemPedido itemPedido)
        {
            await _itemPedidoRepository.AddAsync(itemPedido);
            return itemPedido.Id;
        }

        public async Task AtualizarAsync(ItemPedido itemPedido)
        {
            await _itemPedidoRepository.UpdateAsync(itemPedido);
        }

        public async Task RemoverAsync(int id)
        {
            await _itemPedidoRepository.DeleteAsync(id);
        }
    }
}
