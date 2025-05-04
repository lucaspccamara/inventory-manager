using InventoryManagerApi.Models;
using InventoryManagerApi.Repositories;

namespace InventoryManagerApi.Services
{
    public class MovimentacaoEstoqueService
    {
        private readonly MovimentacaoEstoqueRepository _movimentacaoEstoqueRepository;

        public MovimentacaoEstoqueService(MovimentacaoEstoqueRepository movimentacaoEstoqueRepository)
        {
            _movimentacaoEstoqueRepository = movimentacaoEstoqueRepository;
        }

        public async Task<MovimentacaoEstoque?> ObterPorIdAsync(int id)
        {
            return await _movimentacaoEstoqueRepository.GetByIdAsync(id);
        }

        public async Task<int> AdicionarAsync(MovimentacaoEstoque movimentacaoEstoque)
        {
            await _movimentacaoEstoqueRepository.AddAsync(movimentacaoEstoque);
            return movimentacaoEstoque.Id;
        }

        public async Task AtualizarAsync(MovimentacaoEstoque movimentacaoEstoque)
        {
            await _movimentacaoEstoqueRepository.UpdateAsync(movimentacaoEstoque);
        }

        public async Task RemoverAsync(int id)
        {
            await _movimentacaoEstoqueRepository.DeleteAsync(id);
        }
    }
}
