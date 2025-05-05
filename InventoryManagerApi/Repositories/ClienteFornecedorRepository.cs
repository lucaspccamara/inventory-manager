using InventoryManagerApi.Data;
using InventoryManagerApi.Dtos;
using InventoryManagerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagerApi.Repositories
{
    public class ClienteFornecedorRepository
    {
        private readonly AppDbContext _context;

        public ClienteFornecedorRepository(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<ClienteFornecedor> GetAll()
        {
            return _context.ClientesFornecedores.AsQueryable();
        }

        public async Task<PagedResponse<ClienteFornecedorTableDto>> GetPagedAsync(PagedRequest<ClienteFornecedorFilter> request)
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

            if (!string.IsNullOrEmpty(request.Filter.CpfCnpj))
            {
                query = query.Where(cf => cf.CpfCnpj.Contains(request.Filter.CpfCnpj));
            }

            if (request.Filter.Tipo.HasValue)
            {
                query = query.Where(cf => cf.Tipo == request.Filter.Tipo);
            }

            if (request.Filter.Status.HasValue)
            {
                query = query.Where(cf => cf.Status == request.Filter.Status.Value);
            }

            int totalRecords = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize);

            var data = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(cf => new ClienteFornecedorTableDto
                {
                    Id = cf.Id,
                    Nome = cf.Nome,
                    CpfCnpj = cf.CpfCnpj,
                    Email = cf.Email,
                    Telefone = cf.Telefone,
                    Celular = cf.Celular,
                    Tipo = cf.Tipo,
                    Status = cf.Status
                })
                .OrderBy(cf => cf.Nome)
                .ToListAsync();

            return new PagedResponse<ClienteFornecedorTableDto>
            {
                Data = data,
                TotalRecords = totalRecords,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = totalPages
            };
        }

        public async Task<PagedResponse<ClienteFornecedorSelectDto>> GetPagedToSelectAsync(PagedRequest<ClienteFornecedorFilter> request)
        {
            var query = GetAll();

            if (request.Filter.Id.HasValue)
            {
                query = query.Where(cf => cf.Id == request.Filter.Id.Value);
            }

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

            if (!string.IsNullOrEmpty(request.Filter.CpfCnpj))
            {
                query = query.Where(cf => cf.CpfCnpj.Contains(request.Filter.CpfCnpj));
            }

            if (!string.IsNullOrEmpty(request.Filter.Email))
            {
                query = query.Where(cf => cf.Email.ToLower().Contains(request.Filter.Email.ToLower()));
            }

            if (!string.IsNullOrEmpty(request.Filter.TelefoneCelular))
            {
                query = query.Where(cf => cf.Telefone.Contains(request.Filter.TelefoneCelular) && cf.Celular.Contains(request.Filter.TelefoneCelular));
            }

            if (request.Filter.ListaTipos.Any())
            {
                query = query.Where(cf => request.Filter.ListaTipos.Contains(cf.Tipo));
            }

            if (request.Filter.Status.HasValue)
            {
                query = query.Where(cf => cf.Status == request.Filter.Status.Value);
            }

            int totalRecords = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize);

            var data = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(cf => new ClienteFornecedorSelectDto
                {
                    Id = cf.Id,
                    Nome = cf.Nome
                })
                .OrderBy(cf => cf.Nome)
                .ToListAsync();

            return new PagedResponse<ClienteFornecedorSelectDto>
            {
                Data = data,
                TotalRecords = totalRecords,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = totalPages
            };
        }

        public async Task<ClienteFornecedor?> GetByIdAsync(int id)
        {
            return await _context.ClientesFornecedores.FindAsync(id);
        }

        public async Task AddAsync(ClienteFornecedor clienteFornecedor)
        {
            _context.ClientesFornecedores.Add(clienteFornecedor);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ClienteFornecedor clienteFornecedor)
        {
            _context.ClientesFornecedores.Update(clienteFornecedor);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var clienteFornecedor = await _context.ClientesFornecedores.FindAsync(id);
            if (clienteFornecedor != null)
            {
                clienteFornecedor.Status = false;
                await _context.SaveChangesAsync();
            }
        }
    }
}