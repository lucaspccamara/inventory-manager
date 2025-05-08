using InventoryManagerApi.Dtos;
using InventoryManagerApi.Models;
using InventoryManagerApi.Repositories;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace InventoryManagerApi.Services
{
    public class PedidoService
    {
        private readonly PedidoRepository _pedidoRepository;
        private readonly ItemPedidoRepository _itemPedidoRepository;

        public PedidoService(PedidoRepository pedidoRepository, ItemPedidoRepository itemPedidoRepository)
        {
            _pedidoRepository = pedidoRepository;
            _itemPedidoRepository = itemPedidoRepository;
        }

        public async Task<PagedResponse<PedidoDtoTable>> ListarPedidosAsync(PagedRequest<PedidoFilter> request)
        {
            return await _pedidoRepository.GetPagedAsync(request);
        }

        public async Task<PedidoDto?> ObterPorIdAsync(int id)
        {
            return await _pedidoRepository.GetPedidoDtoByIdAsync(id);
        }

        public async Task<int> AdicionarAsync(PedidoCreateDto pedidoCreateDto)
        {
            var pedido = new Pedido
            {
                ClienteFornecedorId = pedidoCreateDto.ClienteFornecedorId,
                Observacao = pedidoCreateDto.Observacao,
                Data = pedidoCreateDto.Data,
                Status = pedidoCreateDto.Status,
                Total = pedidoCreateDto.Itens.Sum(item => item.Quantidade * item.PrecoUnitario)
            };

            await _pedidoRepository.AddAsync(pedido);

            foreach (var item in pedidoCreateDto.Itens)
            {
                var itemPedido = new ItemPedido
                {
                    PedidoId = pedido.Id,
                    ProdutoId = item.ProdutoId,
                    ProdutoUnidadeVendaId = item.ProdutoUnidadeVendaId,
                    FatorConversao = item.FatorConversao,
                    PrecoUnitario = item.PrecoUnitario,
                    Quantidade = item.Quantidade
                };

                await _itemPedidoRepository.AddAsync(itemPedido);
            }

            return pedido.Id;
        }

        public async Task AtualizarAsync(PedidoUpdateDto pedidoCreateDto)
        {
            var pedido = new Pedido
            {
                Id = pedidoCreateDto.Id.Value,
                ClienteFornecedorId = pedidoCreateDto.ClienteFornecedorId,
                Observacao = pedidoCreateDto.Observacao,
                Data = pedidoCreateDto.Data,
                Status = pedidoCreateDto.Status,
                Total = pedidoCreateDto.Itens.Sum(item => item.Quantidade * item.PrecoUnitario)
            };

            await _pedidoRepository.UpdateAsync(pedido);

            foreach (var item in pedidoCreateDto.ItensAdicionados)
            {
                var itemPedido = new ItemPedido
                {
                    PedidoId = pedido.Id,
                    ProdutoId = item.ProdutoId,
                    ProdutoUnidadeVendaId = item.ProdutoUnidadeVendaId,
                    FatorConversao = item.FatorConversao,
                    PrecoUnitario = item.PrecoUnitario,
                    Quantidade = item.Quantidade
                };

                await _itemPedidoRepository.AddAsync(itemPedido);
            }

            foreach (var item in pedidoCreateDto.ItensModificados)
            {
                var itemPedido = new ItemPedido
                {
                    Id = item.Id.Value,
                    PedidoId = pedido.Id,
                    ProdutoId = item.ProdutoId,
                    ProdutoUnidadeVendaId = item.ProdutoUnidadeVendaId,
                    FatorConversao = item.FatorConversao,
                    PrecoUnitario = item.PrecoUnitario,
                    Quantidade = item.Quantidade
                };

                await _itemPedidoRepository.UpdateAsync(itemPedido);
            }

            foreach (var itemId in pedidoCreateDto.ItensRemovidos)
            {
                await _itemPedidoRepository.DeleteAsync(itemId);
            }
        }

        public async Task AprovarAsync(int id)
        {
            await _pedidoRepository.AprovarAsync(id);
        }

        public async Task CancelarAsync(int id)
        {
            await _pedidoRepository.CancelarAsync(id);
        }

        public async Task RestaurarAsync(int id)
        {
            await _pedidoRepository.RestaurarAsync(id);
        }

        public byte[] GerarPdf(PedidoDto pedido)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    // Cabeçalho moderno
                    page.Header().Element(header =>
                    {
                        header.Column(col =>
    {
                            col.Item().Row(row =>
                            {
                                row.RelativeItem().Column(col2 =>
                                {
                                    col2.Item().Text($"Pedido #{pedido.Id}")
                                        .FontSize(22).Bold().FontColor(Colors.Blue.Medium);
                                    col2.Item().Text($"{(pedido.ClienteFornecedor?.Nome ?? "-")}")
                                        .FontSize(12).FontColor(Colors.Grey.Darken2);
                                    col2.Item().Text($"Data: {pedido.Data:dd/MM/yyyy}")
                                        .FontSize(10).FontColor(Colors.Grey.Darken1);
                                });
                                //row.ConstantItem(80).AlignRight().Element(container =>
                                //{
                                //    container.Height(50)
                                //             .Image("wwwroot/logo.png")
                                //             .FitArea();
                                //}); // Opcional: logo
                            });
                            col.Item().PaddingVertical(8).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                        });
                    });

                    // Conteúdo principal
                    page.Content().Column(col =>
                    {
                        col.Spacing(10);

                        col.Item().Text($"Tipo: {pedido.Status}").FontSize(11).FontColor(Colors.Grey.Darken2);
                        if (!string.IsNullOrWhiteSpace(pedido.Observacao))
                            col.Item().Text($"Observação: {pedido.Observacao}").FontSize(11);

                        // Tabela de produtos
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(5); // Produto
                                columns.RelativeColumn(2); // Unidade
                                columns.RelativeColumn(1); // Quantidade
                                columns.RelativeColumn(2); // Preço Unitário
                                columns.RelativeColumn(2); // Total
                            });

                            // Cabeçalho da tabela
                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Produto").Bold();
                                header.Cell().Element(CellStyle).Text("Unidade").Bold();
                                header.Cell().Element(CellStyle).AlignRight().Text("Qtd").Bold();
                                header.Cell().Element(CellStyle).AlignRight().Text("Preço Unit.").Bold();
                                header.Cell().Element(CellStyle).AlignRight().Text("Total").Bold();
                            });

                            // Linhas dos produtos
                            foreach (var item in pedido.Itens)
                            {
                                table.Cell().Element(CellStyle).Text(item.Nome);
                                table.Cell().Element(CellStyle).Text(item.UnidadeVendaSelecionada?.Nome ?? "-");
                                table.Cell().Element(CellStyle).AlignRight().Text(item.Quantidade.ToString("0.##"));
                                table.Cell().Element(CellStyle).AlignRight().Text(item.PrecoUnitario.ToString("C"));
                                table.Cell().Element(CellStyle).AlignRight().Text((item.Quantidade * item.PrecoUnitario).ToString("C"));
                            }

                            // Função para borda das células
                            IContainer CellStyle(IContainer container) =>
                                container
                                    .Border(1)
                                    .BorderColor(Colors.Grey.Lighten2)
                                    .PaddingVertical(4)
                                    .PaddingHorizontal(6);
                        });

                        // Total do pedido
                        decimal total = pedido.Itens.Sum(i => i.Quantidade * i.PrecoUnitario);
                        col.Item().AlignRight().PaddingTop(10).Text($"Total do Pedido: {total:C}").FontSize(14).Bold().FontColor(Colors.Green.Darken2);
                    });

                    // Rodapé
                    page.Footer().AlignCenter().Text(txt =>
                    {
                        txt.DefaultTextStyle(x => x.FontSize(9).FontColor(Colors.Grey.Darken2));
                        txt.Span("Gerado por Inventory Manager - ");
                        txt.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).FontColor(Colors.Grey.Darken1);
                    });
                });
            });

            return pdf.GeneratePdf();
        }
    }
}
