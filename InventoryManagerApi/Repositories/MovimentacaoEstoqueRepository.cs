using InventoryManagerApi.Data;
using InventoryManagerApi.Models;

namespace InventoryManagerApi.Repositories
{
    public class MovimentacaoEstoqueRepository
    {
        private readonly AppDbContext _context;

        public MovimentacaoEstoqueRepository(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<MovimentacaoEstoque> GetAll()
        {
            return _context.MovimentacoesEstoque.AsQueryable();
        }

        public async Task<MovimentacaoEstoque?> GetByIdAsync(int id)
        {
            return await _context.MovimentacoesEstoque.FindAsync(id);
        }

        public async Task AddAsync(MovimentacaoEstoque movimentacaoEstoque)
        {
            _context.MovimentacoesEstoque.Add(movimentacaoEstoque);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(MovimentacaoEstoque movimentacaoEstoque)
        {
            _context.MovimentacoesEstoque.Update(movimentacaoEstoque);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var movimentacaoEstoque = await _context.MovimentacoesEstoque.FindAsync(id);
            if (movimentacaoEstoque != null)
            {
                _context.MovimentacoesEstoque.Remove(movimentacaoEstoque);
                await _context.SaveChangesAsync();
            }
        }
    }
}
