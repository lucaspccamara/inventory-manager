using InventoryManagerApi.Dtos;
using InventoryManagerApi.Models;
using InventoryManagerApi.Repositories;

namespace InventoryManagerApi.Services
{
    public class ProdutoService
    {
        private readonly ProdutoRepository _repository;
        private readonly ProdutoUnidadeVendaRepository _produtoUnidadeVendaRepository;

        public ProdutoService(ProdutoRepository repository, ProdutoUnidadeVendaRepository produtoUnidadeVendaRepository)
        {
            _repository = repository;
            _produtoUnidadeVendaRepository = produtoUnidadeVendaRepository;
        }

        public async Task<PagedResponse<ProdutoDtoTable>> ListarProdutosAsync(PagedRequest<ProdutoFilter> request)
        {
            var produtos = await _repository.GetPagedAsync(request);

            // Carregar unidades de venda para cada produto
            foreach (var produto in produtos.Data)
            {
                var unidadesVenda = await _produtoUnidadeVendaRepository.GetByProdutoIdAsync(produto.Id);

                // Ordenar as unidades de venda pelo fator (do maior para o menor)
                var unidadesOrdenadas = unidadesVenda
                    .OrderByDescending(u => u.FatorConversao)
                    .ToList();

                var restante = produto.Quantidade;
                var resultados = new List<QuantidadePorUnidadeDto>();

                foreach (var unidade in unidadesOrdenadas)
                {
                    int quantidade = (int)(restante / unidade.FatorConversao);
                    restante = (int)(restante % unidade.FatorConversao);

                    resultados.Add(new QuantidadePorUnidadeDto
                    {
                        Unidade = $"{unidade.UnidadeMedida.Nome} ({unidade.UnidadeMedida.Sigla})",
                        Quantidade = quantidade
                    });
                }

                // Adicionar o restante na menor unidade
                if (restante > 0)
                {
                    var menorUnidade = unidadesVenda.FirstOrDefault(u => u.FatorConversao == 1);
                    if (menorUnidade != null)
                    {
                        resultados.Add(new QuantidadePorUnidadeDto
                        {
                            Unidade = $"{menorUnidade.UnidadeMedida.Nome} ({menorUnidade.UnidadeMedida.Sigla})",
                            Quantidade = restante
                        });
                    }
                }

                produto.QuantidadesPorUnidade = resultados;
            }

            return produtos;
        }

        public async Task<ProdutoDto?> ObterPorIdAsync(int id)
        {
            return await _repository.GetProdutoDtoByIdAsync(id);
        }

        public async Task<int> AdicionarAsync(ProdutoCreateDto produtoCreateDto)
        {
            var produto = new Produto
            {
                Nome = produtoCreateDto.Nome,
                Descricao = produtoCreateDto.Descricao,
                UnidadeMedidaCompraId = produtoCreateDto.UnidadeCompraId,
                MenorUnidadeId = produtoCreateDto.MenorUnidadeId,
                Quantidade = produtoCreateDto.Quantidade,
                Status = produtoCreateDto.Status
            };

            await _repository.AddAsync(produto);

            foreach (var unidadeVendaDto in produtoCreateDto.UnidadesVenda)
            {
                var unidadeVenda = new ProdutoUnidadeVenda
                {
                    ProdutoId = produto.Id,
                    UnidadeMedidaId = unidadeVendaDto.UnidadeMedidaId,
                    MenorUnidadeId = unidadeVendaDto.MenorUnidadeId,
                    FatorConversao = unidadeVendaDto.Fator,
                    PrecoPadrao = unidadeVendaDto.PrecoPadrao,
                    Status = unidadeVendaDto.Status
                };

                await _produtoUnidadeVendaRepository.AddAsync(unidadeVenda);
            }

            await _produtoUnidadeVendaRepository.SaveChangesAsync();

            return produto.Id;
        }

        public async Task AtualizarAsync(ProdutoCreateDto produtoUpdateDto)
        {
            var produto = await _repository.GetByIdAsync(produtoUpdateDto.Id.Value);
            if (produto == null)
            {
                throw new KeyNotFoundException("Produto não encontrado.");
            }

            produto.Nome = produtoUpdateDto.Nome;
            produto.Descricao = produtoUpdateDto.Descricao;
            produto.UnidadeMedidaCompraId = produtoUpdateDto.UnidadeCompraId;
            produto.MenorUnidadeId = produtoUpdateDto.MenorUnidadeId;
            produto.Quantidade = produtoUpdateDto.Quantidade;
            produto.Status = produtoUpdateDto.Status;

            await _repository.UpdateAsync(produto);

            // Obter unidades de venda existentes
            var unidadesVendaExistentes = await _produtoUnidadeVendaRepository.GetByProdutoIdAsync(produto.Id);

            // Desativar unidades de venda que foram removidas ou alteradas
            foreach (var unidadeVenda in unidadesVendaExistentes)
            {
                var unidadeVendaDto = produtoUpdateDto.UnidadesVenda.FirstOrDefault(uv => uv.Id == unidadeVenda.Id);
                if (unidadeVendaDto == null ||
                    unidadeVendaDto.UnidadeMedidaId != unidadeVenda.UnidadeMedidaId ||
                    unidadeVendaDto.MenorUnidadeId != unidadeVenda.MenorUnidadeId ||
                    unidadeVendaDto.Fator != unidadeVenda.FatorConversao ||
                    unidadeVendaDto.PrecoPadrao != unidadeVenda.PrecoPadrao)
                {
                    unidadeVenda.Status = false;
                    await _produtoUnidadeVendaRepository.UpdateAsync(unidadeVenda);

                    // Preparando para inserção da nova unidade de venda
                    produtoUpdateDto.UnidadesVenda.First(uv => uv.Id == unidadeVenda.Id).Id = null;
                }
            }

            // Adicionar novas unidades de venda
            foreach (var unidadeVendaDto in produtoUpdateDto.UnidadesVenda)
            {
                if (unidadeVendaDto.Id == null || !unidadesVendaExistentes.Any(uv => uv.Id == unidadeVendaDto.Id))
                {
                    var unidadeVenda = new ProdutoUnidadeVenda
                    {
                        ProdutoId = produto.Id,
                        UnidadeMedidaId = unidadeVendaDto.UnidadeMedidaId,
                        MenorUnidadeId = unidadeVendaDto.MenorUnidadeId,
                        FatorConversao = unidadeVendaDto.Fator,
                        PrecoPadrao = unidadeVendaDto.PrecoPadrao,
                        Status = true
                    };

                    await _produtoUnidadeVendaRepository.AddAsync(unidadeVenda);
                }
            }

            await _produtoUnidadeVendaRepository.SaveChangesAsync();
        }

        public async Task RemoverAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
