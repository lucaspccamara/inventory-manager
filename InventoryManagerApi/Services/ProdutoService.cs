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

        //public async Task<PagedResponse<UnidadeMedidaDto>> ListarUnidadesAsync(PagedRequest<UnidadeMedidaFilter> request)
        //{
        //    return await _repository.GetPagedAsync(request);
        //}

        public async Task<ProdutoDto?> ObterPorIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task AdicionarAsync(ProdutoDto produtoDto)
        {
            var produto = new Produto
            {
                Nome = produtoDto.Nome,
                Descricao = produtoDto.Descricao,
                UnidadeMedidaCompraId = produtoDto.UnidadeCompraId,
                MenorUnidadeId = produtoDto.MenorUnidadeId,
                Status = produtoDto.Status
            };

            await _repository.AddAsync(produto);

            foreach (var unidadeVendaDto in produtoDto.UnidadesVenda)
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
        }

        public async Task AtualizarAsync(Produto unidade)
        {
            await _repository.UpdateAsync(unidade);
        }

        public async Task RemoverAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
