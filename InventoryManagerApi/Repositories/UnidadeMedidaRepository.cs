using InventoryManagerApi.Data;
using InventoryManagerApi.Dtos;
using InventoryManagerApi.Models;
using InventoryManagerApi.Utils;
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

        public async Task<PagedResponse<UnidadeMedidaDto>> GetPagedAsync(PagedRequest<UnidadeFilter> request)
        {
            var query = GetAll();

            if (request.Filter.Status.HasValue)
            {
                query = query.Where(u => u.Status == request.Filter.Status.Value);
            }

            if (!string.IsNullOrEmpty(request.Filter.Sigla))
            {
                query = query.Where(u => u.Sigla.Contains(request.Filter.Sigla));
            }

            // Executa a query e trás para a memória
            var data = await query.ToListAsync();

            // Aplica a lógica de tipo de busca
            data = data
                .Where(u => string.IsNullOrEmpty(request.Filter.Nome) || SearchUtils.ApplySearchType(u.Nome, request.Filter.Nome, request.Filter.SearchType))
                .ToList();

            int totalRecords = data.Count;
            int totalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize);

            var pagedData = data
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(u => new UnidadeMedidaDto
                {
                    Id = u.Id,
                    Nome = u.Nome,
                    Sigla = u.Sigla,
                    Status = u.Status
                })
                .ToList();

            return new PagedResponse<UnidadeMedidaDto>
            {
                Data = pagedData,
                TotalRecords = totalRecords,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = totalPages
            };
        }

        public async Task<UnidadeMedidaDto?> GetByIdAsync(int id)
        {
            var unidade = await _context.UnidadesMedida.FindAsync(id);
            if (unidade == null)
            {
                return null;
            }

            return new UnidadeMedidaDto
            {
                Id = unidade.Id,
                Nome = unidade.Nome,
                Sigla = unidade.Sigla,
                Status = unidade.Status
            };
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
