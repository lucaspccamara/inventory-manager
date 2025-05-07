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
                .OrderByDescending(p => p.Id)
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
            var pedidoDto = await _context.Pedidos
            .Where(p => p.Id == id)
            .Select(p => new PedidoDto
            {
                Id = p.Id,
                ClienteFornecedor = new ClienteFornecedorSelectDto
                {
                    Id = p.ClienteFornecedor.Id,
                    Nome = p.ClienteFornecedor.Nome
                },
                Data = p.Data,
                Status = p.Status,
                Observacao = p.Observacao,
                Itens = p.Itens.Select(i => new ItemPedidoDto
                {
                    Id = i.Id,
                    PedidoId = i.PedidoId,
                    ProdutoId = i.ProdutoId,
                    Nome = i.Produto.Nome,
                    Quantidade = i.Quantidade,
                    UnidadesVenda = i.Produto.UnidadesVenda.Select(uv => new ProdutoUnidadeVendaDto
                    {
                        Id = uv.Id,
                        ProdutoId = uv.ProdutoId,
                        Origem = new UnidadeMedidaDto
                        {
                            Id = uv.UnidadeMedida.Id,
                            Nome = uv.UnidadeMedida.Nome,
                            Sigla = uv.UnidadeMedida.Sigla,
                            Status = uv.UnidadeMedida.Status
                        },
                        Fator = uv.FatorConversao,
                        PrecoPadrao = uv.PrecoPadrao
                    }).ToList(),
                    UnidadeVendaSelecionada = i.Produto.UnidadesVenda
                        .Where(uv => uv.Id == i.ProdutoUnidadeVendaId)
                        .Select(uv => new UnidadeMedidaDto
                        {
                            Id = uv.Id,
                            Nome = uv.UnidadeMedida.Nome
                        }).First(),
                    PrecoUnitario = i.PrecoUnitario
                }).ToList()
            })
            .FirstOrDefaultAsync();

            if (pedidoDto == null)
            {
                return null;
            }

            return pedidoDto;
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

        public async Task CancelarAsync(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            var tipoMovimentacao = ETipoMovimentacao.Entrada;

            if (pedido != null)
            {
                if (pedido.Status == EStatusPedido.Orçamento)
                {
                    pedido.Status = EStatusPedido.CanceladoOrçamento;
                }
                else if (pedido.Status == EStatusPedido.Venda)
                {
                    pedido.Status = EStatusPedido.CanceladoVenda;
                }
                else
                {
                    pedido.Status = EStatusPedido.CanceladoCompra;
                    tipoMovimentacao = ETipoMovimentacao.Saida;
                }

                _context.Pedidos.Update(pedido);
                await _context.SaveChangesAsync();

                if (pedido.Status == EStatusPedido.CanceladoVenda || pedido.Status == EStatusPedido.CanceladoCompra)
                {
                    var itensPedido = await _context.ItensPedido
                        .Where(i => i.PedidoId == pedido.Id)
                        .ToListAsync();

                    foreach (var item in itensPedido)
                    {
                        var movimentacao = new MovimentacaoEstoque
                        {
                            ProdutoId = item.ProdutoId,
                            PedidoId = item.PedidoId,
                            Quantidade = (int)(item.Quantidade * item.FatorConversao),
                            Tipo = tipoMovimentacao
                        };
                        await _context.MovimentacoesEstoque.AddAsync(movimentacao);

                        var produto = await _context.Produtos.FindAsync(item.ProdutoId);
                        produto?.AtualizarEstoque(movimentacao.Quantidade, movimentacao.Tipo);
                        await _context.SaveChangesAsync();
                    }
                }
            }
        }
    }
}
