﻿using InventoryManagerApi.Dtos;
using InventoryManagerApi.Models;
using InventoryManagerApi.Repositories;

namespace InventoryManagerApi.Services
{
    public class UnidadeMedidaService
    {
        private readonly UnidadeMedidaRepository _repository;

        public UnidadeMedidaService(UnidadeMedidaRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResponse<UnidadeMedidaDto>> ListarUnidadesAsync(PagedRequest<UnidadeMedidaFilter> request)
        {
            return await _repository.GetPagedAsync(request);
        }

        public async Task<UnidadeMedidaDto?> ObterPorIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task AdicionarAsync(UnidadeMedida unidade)
        {
            await _repository.AddAsync(unidade);
        }

        public async Task AtualizarAsync(UnidadeMedida unidade)
        {
            await _repository.UpdateAsync(unidade);
        }

        public async Task RemoverAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
