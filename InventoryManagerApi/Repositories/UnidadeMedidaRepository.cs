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

        public async Task<PagedResponse<UnidadeMedidaDto>> GetPagedAsync(PagedRequest<UnidadeMedidaFilter> request)
        {
            var query = GetAll();

            if (!string.IsNullOrEmpty(request.Filter.Nome))
            {
                if (request.Filter.SearchType == "contains")
                    query = query.Where(cf => cf.Nome.ToLower().Contains(request.Filter.Nome.ToLower()));
                else if (request.Filter.SearchType == "startsWith")
                    query = query.Where(cf => cf.Nome.ToLower().StartsWith(request.Filter.Nome.ToLower()));
                else if (request.Filter.SearchType == "endsWith")
                    query = query.Where(cf => cf.Nome.ToLower().EndsWith(request.Filter.Nome.ToLower()));
                else if (request.Filter.SearchType == "equals")
                    query = query.Where(cf => cf.Nome.ToLower().Equals(request.Filter.Nome.ToLower()));
            }

            if (request.Filter.Status.HasValue)
            {
                query = query.Where(u => u.Status == request.Filter.Status.Value);
            }

            if (!string.IsNullOrEmpty(request.Filter.Sigla))
            {
                query = query.Where(u => u.Sigla.Contains(request.Filter.Sigla));
            }

            int totalRecords = await query.CountAsync();

            var data = new List<UnidadeMedidaDto>();

            if (request.IsAll)
            {
                data = await query
                .OrderBy(u => u.Nome)
                .Select(u => new UnidadeMedidaDto
                {
                    Id = u.Id,
                    Nome = u.Nome,
                    Sigla = u.Sigla,
                    Status = u.Status
                })
                .ToListAsync();
            }
            else
            {
                data = await query
                    .OrderBy(u => u.Nome)
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(u => new UnidadeMedidaDto
                    {
                        Id = u.Id,
                        Nome = u.Nome,
                        Sigla = u.Sigla,
                        Status = u.Status
                    })
                    .ToListAsync();
            }

            return new PagedResponse<UnidadeMedidaDto>
            {
                Data = data,
                TotalRecords = totalRecords,
                Page = request.Page,
                PageSize = request.PageSize
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
