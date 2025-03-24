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

        //public async Task<PagedResponse<UnidadeMedidaDto>> GetPagedAsync(PagedRequest<UnidadeMedidaFilter> request)
        //{
        //    var query = GetAll();

        //    if (request.Filter.Status.HasValue)
        //    {
        //        query = query.Where(u => u.Status == request.Filter.Status.Value);
        //    }

        //    if (!string.IsNullOrEmpty(request.Filter.Sigla))
        //    {
        //        query = query.Where(u => u.Sigla.Contains(request.Filter.Sigla));
        //    }

        //    // Executa a query e trás para a memória
        //    var data = await query.ToListAsync();

        //    // Aplica a lógica de tipo de busca
        //    data = data
        //        .Where(u => string.IsNullOrEmpty(request.Filter.Nome) || SearchUtils.ApplySearchType(u.Nome, request.Filter.Nome, request.Filter.SearchType))
        //        .ToList();

        //    int totalRecords = data.Count;
        //    int totalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize);

        //    var pagedData = data
        //        .Skip((request.Page - 1) * request.PageSize)
        //        .Take(request.PageSize)
        //        .Select(u => new UnidadeMedidaDto
        //        {
        //            Id = u.Id,
        //            Nome = u.Nome,
        //            Sigla = u.Sigla,
        //            Status = u.Status
        //        })
        //        .ToList();

        //    return new PagedResponse<UnidadeMedidaDto>
        //    {
        //        Data = pagedData,
        //        TotalRecords = totalRecords,
        //        Page = request.Page,
        //        PageSize = request.PageSize,
        //        TotalPages = totalPages
        //    };
        //}

        public async Task<ProdutoDto?> GetByIdAsync(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                return null;
            }

            return new ProdutoDto
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Descricao = produto.Descricao,
                UnidadeCompraId = produto.UnidadeMedidaCompraId,
                MenorUnidadeId = produto.MenorUnidadeId,
                UnidadesVenda = produto.UnidadesVenda.Select(uv => new ProdutoUnidadeVendaDto
                {
                    Id = uv.Id,
                    ProdutoId = uv.ProdutoId,
                    UnidadeMedidaId = uv.UnidadeMedidaId,
                    MenorUnidadeId = uv.MenorUnidadeId,
                    Fator = uv.FatorConversao
                }).ToList(),
                Status = produto.Status
            };
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
