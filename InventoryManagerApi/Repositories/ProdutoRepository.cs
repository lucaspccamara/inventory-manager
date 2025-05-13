using InventoryManagerApi.Data;
using InventoryManagerApi.Dtos;
using InventoryManagerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagerApi.Repositories
{
    public class ProdutoRepository
    {
        private readonly AppDbContext _context;

        public ProdutoRepository(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<Produto> GetAll()
        {
            return _context.Produtos.AsQueryable();
        }

        public async Task<PagedResponse<ProdutoDtoTable>> GetPagedAsync(PagedRequest<ProdutoFilter> request)
        {
            var query = GetAll();

            if (request.Filter.Id.HasValue)
            {
                query = query.Where(u => u.Id == request.Filter.Id.Value);
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

            if (request.Filter.Status.HasValue)
            {
                query = query.Where(u => u.Status == request.Filter.Status.Value);
            }

            int totalRecords = await query.CountAsync();

            var data = new List<ProdutoDtoTable>();

            if (request.IsAll)
            {
                data = await query
                    .OrderBy(p => p.Nome)
                    .Select(u => new ProdutoDtoTable
                    {
                        Id = u.Id,
                        Nome = u.Nome,
                        Quantidade = u.Quantidade,
                        Status = u.Status
                    })
                    .ToListAsync();
            }
            else
            {
                data = await query
                    .OrderBy(p => p.Nome)
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(u => new ProdutoDtoTable
                    {
                        Id = u.Id,
                        Nome = u.Nome,
                        Quantidade = u.Quantidade,
                        Status = u.Status
                    })
                    .ToListAsync();
            }

            return new PagedResponse<ProdutoDtoTable>
            {
                Data = data,
                TotalRecords = totalRecords,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }

        public async Task<ProdutoDto?> GetProdutoDtoByIdAsync(int id)
        {
            var produto = await _context.Produtos
                .Include(p => p.UnidadeMedidaCompra)
                .Include(p => p.MenorUnidade)
                .Include(p => p.UnidadesVenda)
                .ThenInclude(uv => uv.UnidadeMedida)
                .Include(p => p.UnidadesVenda)
                .ThenInclude(uv => uv.MenorUnidade)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (produto == null)
            {
                return null;
            }

            return new ProdutoDto
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Descricao = produto.Descricao,
                Quantidade = produto.Quantidade,
                UnidadeCompra = new UnidadeMedidaDto
                {
                    Id = produto.UnidadeMedidaCompra.Id,
                    Nome = produto.UnidadeMedidaCompra.Nome,
                    Sigla = produto.UnidadeMedidaCompra.Sigla,
                    Status = produto.UnidadeMedidaCompra.Status
                },
                MenorUnidade = new UnidadeMedidaDto
                {
                    Id = produto.MenorUnidade.Id,
                    Nome = produto.MenorUnidade.Nome,
                    Sigla = produto.MenorUnidade.Sigla,
                    Status = produto.MenorUnidade.Status
                },
                UnidadesVenda = produto.UnidadesVenda.Select(uv => new ProdutoUnidadeVendaDto
                {
                    Id = uv.Id,
                    ProdutoId = uv.ProdutoId,
                    Fator = uv.FatorConversao,
                    PrecoPadrao = uv.PrecoPadrao,
                    Status = uv.Status,
                    Origem = new UnidadeMedidaDto
                    {
                        Id = uv.UnidadeMedida.Id,
                        Nome = uv.UnidadeMedida.Nome,
                        Sigla = uv.UnidadeMedida.Sigla,
                        Status = uv.UnidadeMedida.Status
                    },
                    Destino = new UnidadeMedidaDto
                    {
                        Id = uv.MenorUnidade.Id,
                        Nome = uv.MenorUnidade.Nome,
                        Sigla = uv.MenorUnidade.Sigla,
                        Status = uv.MenorUnidade.Status
                    }
                }).Where(uv => uv.Status).OrderByDescending(un => un.Fator).ToList(),
                Status = produto.Status
            };
        }

        public async Task<Produto?> GetByIdAsync(int id)
        {
            return await _context.Produtos.FindAsync(id);
        }

        public async Task AddAsync(Produto produto)
        {
            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Produto produto)
        {
            _context.Produtos.Update(produto);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto != null)
            {
                produto.Status = false;
                await _context.SaveChangesAsync();
            }
        }
    }
}
