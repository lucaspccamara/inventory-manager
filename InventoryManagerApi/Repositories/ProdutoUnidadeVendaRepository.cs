using InventoryManagerApi.Data;
using InventoryManagerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagerApi.Repositories
{
    public class ProdutoUnidadeVendaRepository
    {
        private readonly AppDbContext _context;

        public ProdutoUnidadeVendaRepository(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<ProdutoUnidadeVenda> GetAll()
        {
            return _context.ProdutosUnidadeVenda.AsQueryable();
        }

        public async Task<ProdutoUnidadeVenda?> GetByIdAsync(int id)
        {
            return await _context.ProdutosUnidadeVenda.FindAsync(id);
        }

        public async Task AddAsync(ProdutoUnidadeVenda produtoUnidadeVenda)
        {
            await _context.ProdutosUnidadeVenda.AddAsync(produtoUnidadeVenda);
        }

        public async Task UpdateAsync(ProdutoUnidadeVenda produtoUnidadeVenda)
        {
            _context.ProdutosUnidadeVenda.Update(produtoUnidadeVenda);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var produtoUnidadeVenda = await _context.ProdutosUnidadeVenda.FindAsync(id);
            if (produtoUnidadeVenda != null)
            {
                _context.ProdutosUnidadeVenda.Remove(produtoUnidadeVenda);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
