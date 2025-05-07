using InventoryManagerApi.Data;
using InventoryManagerApi.Enums;
using InventoryManagerApi.Models;

namespace InventoryManagerApi.Repositories
{
    public class ItemPedidoRepository
    {
        private readonly AppDbContext _context;
        private readonly MovimentacaoEstoqueRepository _movimentacaoEstoqueRepository;

        public ItemPedidoRepository(AppDbContext context, MovimentacaoEstoqueRepository movimentacaoEstoqueRepository)
        {
            _context = context;
            _movimentacaoEstoqueRepository = movimentacaoEstoqueRepository;
        }

        public IQueryable<ItemPedido> GetAll()
        {
            return _context.ItensPedido.AsQueryable();
        }

        public async Task<ItemPedido?> GetByIdAsync(int id)
        {
            return await _context.ItensPedido.FindAsync(id);
        }

        public async Task AddAsync(ItemPedido itemPedido)
        {
            _context.ItensPedido.Add(itemPedido);
            await _context.SaveChangesAsync();

            var pedido = await _context.Pedidos.FindAsync(itemPedido.PedidoId);
            if (pedido != null && (pedido.Status == EStatusPedido.Venda || pedido.Status == EStatusPedido.Compra))
            {
                var movimentacao = new MovimentacaoEstoque
                {
                    ProdutoId = itemPedido.ProdutoId,
                    PedidoId = itemPedido.PedidoId,
                    Quantidade = (int)(itemPedido.Quantidade * itemPedido.FatorConversao),
                    Tipo = pedido.Status == EStatusPedido.Venda ? ETipoMovimentacao.Saida : ETipoMovimentacao.Entrada
                };
                await _movimentacaoEstoqueRepository.AddAsync(movimentacao);

                var produto = await _context.Produtos.FindAsync(itemPedido.ProdutoId);
                produto?.AtualizarEstoque(movimentacao.Quantidade, movimentacao.Tipo);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(ItemPedido itemPedido)
        {
            var itemPedidoExistente = await _context.ItensPedido.FindAsync(itemPedido.Id);
            if (itemPedidoExistente == null)
            {
                throw new KeyNotFoundException("Item do pedido não encontrado.");
            }

            var pedido = await _context.Pedidos.FindAsync(itemPedido.PedidoId);
            if (pedido == null)
            {
                throw new KeyNotFoundException("Pedido não encontrado.");
            }

            // Calcular a diferença de quantidade
            int diferencaQuantidade = itemPedido.Quantidade - itemPedidoExistente.Quantidade;

            // Atualizar o item do pedido
            itemPedidoExistente.ProdutoId = itemPedido.ProdutoId;
            itemPedidoExistente.ProdutoUnidadeVendaId = itemPedido.ProdutoUnidadeVendaId;
            itemPedidoExistente.Quantidade = itemPedido.Quantidade;
            itemPedidoExistente.PrecoUnitario = itemPedido.PrecoUnitario;

            // Registrar movimentação de estoque com base na diferença de quantidade
            if (diferencaQuantidade != 0)
            {
                var movimentacao = new MovimentacaoEstoque
                {
                    ProdutoId = itemPedido.ProdutoId,
                    PedidoId = itemPedido.PedidoId,
                    Quantidade = (int)(Math.Abs(diferencaQuantidade) * itemPedido.FatorConversao),
                    Tipo = diferencaQuantidade > 0
                    ? (pedido.Status == EStatusPedido.Venda ? ETipoMovimentacao.Saida : ETipoMovimentacao.Entrada)
                        : (pedido.Status == EStatusPedido.Venda ? ETipoMovimentacao.Entrada : ETipoMovimentacao.Saida)
                };
                await _movimentacaoEstoqueRepository.AddAsync(movimentacao);

                var produto = await _context.Produtos.FindAsync(itemPedido.ProdutoId);
                produto?.AtualizarEstoque(movimentacao.Quantidade, movimentacao.Tipo);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var itemPedido = await _context.ItensPedido.FindAsync(id);
            if (itemPedido != null)
            {
                var pedido = await _context.Pedidos.FindAsync(itemPedido.PedidoId);
                if (pedido != null && (pedido.Status == EStatusPedido.Venda || pedido.Status == EStatusPedido.Compra))
                {
                    var movimentacao = new MovimentacaoEstoque
                    {
                        ProdutoId = itemPedido.ProdutoId,
                        PedidoId = itemPedido.PedidoId,
                        Quantidade = (int)(itemPedido.Quantidade * itemPedido.FatorConversao),
                        Tipo = pedido.Status == EStatusPedido.Venda ? ETipoMovimentacao.Entrada : ETipoMovimentacao.Saida
                    };
                    await _movimentacaoEstoqueRepository.AddAsync(movimentacao);

                    var produto = await _context.Produtos.FindAsync(itemPedido.ProdutoId);
                    produto?.AtualizarEstoque(movimentacao.Quantidade, movimentacao.Tipo);
                }

                _context.ItensPedido.Remove(itemPedido);
                await _context.SaveChangesAsync();
            }
        }
    }
}
