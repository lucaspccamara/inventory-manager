using InventoryManagerApi.Data;
using InventoryManagerApi.Dtos;
using InventoryManagerApi.Models;
using InventoryManagerApi.Utils;
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
                query = query.Where(cf => cf.Nome.Contains(request.Filter.Nome));
            }

            if (!string.IsNullOrEmpty(request.Filter.CpfCnpj))
            {
                query = query.Where(cf => cf.CpfCnpj.Contains(request.Filter.CpfCnpj));
            }

            if (request.Filter.Tipo.HasValue)
            {
                query = query.Where(cf => cf.Tipo == request.Filter.Tipo.Value);
            }

            if (request.Filter.Status.HasValue)
            {
                query = query.Where(cf => cf.Status == request.Filter.Status.Value);
            }

            // Executa a query e trás para a memória
            var data = await query.ToListAsync();

            // Aplica a lógica de tipo de busca
            data = data
                .Where(u => string.IsNullOrEmpty(request.Filter.Nome) || SearchUtils.ApplySearchType(u.Nome, request.Filter.Nome, request.Filter.SearchType))
                .ToList();

            int totalRecords = data.Count();
            int totalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize);

            var pagedData = data
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
                }).ToList();

            return new PagedResponse<ClienteFornecedorTableDto>
            {
                Data = pagedData,
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