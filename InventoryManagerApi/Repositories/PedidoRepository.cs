using InventoryManagerApi.Data;
using InventoryManagerApi.Dtos;
using InventoryManagerApi.Enums;
using InventoryManagerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagerApi.Repositories
{
    public class PedidoRepository
    {
        private readonly AppDbContext _context;

        public PedidoRepository(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<Pedido> GetAll()
        {
            return _context.Pedidos.AsQueryable();
        }

        public async Task<PagedResponse<PedidoDtoTable>> GetPagedAsync(PagedRequest<PedidoFilter> request)
        {
            var query = GetAll();

            query.Include(p => p.ClienteFornecedor);

            if (request.Filter.Id.HasValue)
            {
                query = query.Where(p => p.Id == request.Filter.Id.Value);
            }

            if (request.Filter.ClienteFornecedorId.HasValue)
            {
                query = query.Where(p => p.ClienteFornecedorId == request.Filter.ClienteFornecedorId.Value);
            }

            if (request.Filter.DataInicio != DateTime.MinValue && request.Filter.DataFim != DateTime.MinValue)
            {
                query = query.Where(p => p.Data >= request.Filter.DataInicio && p.Data <= request.Filter.DataFim);
            }

            if (request.Filter.Status.HasValue)
            {
                if (request.Filter.Status.Value == EStatusPedido.CanceladoVenda)
                {
                    // Filtra para CanceladoVenda ou CanceladoOrçamento
                    query = query.Where(p => p.Status == EStatusPedido.CanceladoVenda || p.Status == EStatusPedido.CanceladoOrçamento);
                }
                else
                {
                    // Filtra para o status específico
                    query = query.Where(p => p.Status == request.Filter.Status.Value);
                }
            } else if (request.Filter.Tipo == ETipoMovimentacao.Entrada)
            {
                query = query.Where(p => p.Status >= EStatusPedido.Compra);
            } else
            {
                query = query.Where(p => p.Status <= EStatusPedido.CanceladoVenda);
            }

            int totalRecords = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize);

            var data = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(p => new PedidoDtoTable
                {
                    Id = p.Id,
                    ClienteFornecedorNome = p.ClienteFornecedor.Nome,
                    Data = p.Data,
                    Status = p.Status,
                    Total = p.Total
                })
                .ToListAsync();

            return new PagedResponse<PedidoDtoTable>
            {
                Data = data,
                TotalRecords = totalRecords,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = totalPages
            };
        }

        public async Task<PedidoDto?> GetPedidoDtoByIdAsync(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.ClienteFornecedor)
                .Include(p => p.Itens)
                .ThenInclude(i => i.Produto)
                .Include(p => p.Itens)
                .ThenInclude(i => i.ProdutoUnidadeVenda)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null)
            {
                return null;
            }

            return new PedidoDto
            {
                Id = pedido.Id,
                ClienteFornecedorId = pedido.ClienteFornecedorId,
                ClienteFornecedorNome = pedido.ClienteFornecedor.Nome,
                Data = pedido.Data,
                Status = pedido.Status,
                Observacao = pedido.Observacao,
                Total = pedido.Total,
                Itens = pedido.Itens.Select(i => new ItemPedidoDto
                {
                    Id = i.Id,
                    ProdutoId = i.ProdutoId,
                    ProdutoNome = i.Produto.Nome,
                    ProdutoUnidadeVendaId = i.ProdutoUnidadeVendaId,
                    UnidadeMedidaNome = i.ProdutoUnidadeVenda.UnidadeMedida.Nome,
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.PrecoUnitario
                }).ToList()
            };
        }

        public async Task<Pedido?> GetByIdAsync(int id)
        {
            return await _context.Pedidos.FindAsync(id);
        }

        public async Task AddAsync(Pedido pedido)
        {
            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Pedido pedido)
        {
            _context.Pedidos.Update(pedido);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido != null)
            {
                _context.Pedidos.Remove(pedido);
                await _context.SaveChangesAsync();
            }
        }
    }
}
