using InventoryManagerApi.Data;
using InventoryManagerApi.Dtos;
using InventoryManagerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagerApi.Repositories
{
    public class UnidadeMedidaRepository
    {
        private readonly AppDbContext _context;

        public UnidadeMedidaRepository(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<UnidadeMedida> GetAll()
        {
            return _context.UnidadesMedida.AsQueryable();
        }

        public async Task<PagedResponse<UnidadeMedida>> GetPagedAsync(PagedRequest<UnidadeFilter> request)
        {
            var query = GetAll()
                .Where(u => string.IsNullOrEmpty(request.Filter.Nome) || u.Nome.Contains(request.Filter.Nome))
                .Where(u => string.IsNullOrEmpty(request.Filter.Sigla) || u.Sigla.Contains(request.Filter.Sigla))
                .Where(u => request.Filter.Status == null || u.Status == request.Filter.Status);

            int totalRecords = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize);

            var data = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PagedResponse<UnidadeMedida>
            {
                Data = data,
                TotalRecords = totalRecords,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = totalPages
            };
        }

        public async Task<UnidadeMedida?> GetByIdAsync(int id)
        {
            return await _context.UnidadesMedida.FindAsync(id);
        }

        public async Task AddAsync(UnidadeMedida unidade)
        {
            _context.UnidadesMedida.Add(unidade);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(UnidadeMedida unidade)
        {
            _context.UnidadesMedida.Update(unidade);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var unidade = await _context.UnidadesMedida.FindAsync(id);
            if (unidade != null)
            {
                unidade.Status = false;
                await _context.SaveChangesAsync();
            }
        }
    }
}
