using InventoryManagerApi.Data;
using InventoryManagerApi.Dtos;
using InventoryManagerApi.Models;
using InventoryManagerApi.Utils;
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

            if (request.Filter.Status.HasValue)
            {
                query = query.Where(u => u.Status == request.Filter.Status.Value);
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
                .Select(u => new ProdutoDtoTable
                {
                    Id = u.Id,
                    Nome = u.Nome,
                    Quantidade = u.Quantidade,
                    Status = u.Status
                })
                .ToList();

            return new PagedResponse<ProdutoDtoTable>
            {
                Data = pagedData,
                TotalRecords = totalRecords,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = totalPages
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
