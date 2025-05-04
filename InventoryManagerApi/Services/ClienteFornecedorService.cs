using InventoryManagerApi.Dtos;
using InventoryManagerApi.Models;
using InventoryManagerApi.Repositories;

namespace InventoryManagerApi.Services
{
    public class ClienteFornecedorService
    {
        private readonly ClienteFornecedorRepository _repository;

        public ClienteFornecedorService(ClienteFornecedorRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResponse<ClienteFornecedorTableDto>> ListarClienteFornecedorAsync(PagedRequest<ClienteFornecedorFilter> request)
        {
            return await _repository.GetPagedAsync(request);
        }

        public async Task<PagedResponse<ClienteFornecedorSelectDto>> ListarClienteFornecedorSelectAsync(PagedRequest<ClienteFornecedorFilter> request)
        {
            return await _repository.GetPagedToSelectAsync(request);
        }

        public async Task<ClienteFornecedorDto?> ObterPorIdAsync(int id)
        {
            var clienteFornecedor = await _repository.GetByIdAsync(id);
            if (clienteFornecedor == null) return null;

            return new ClienteFornecedorDto
            {
                Id = clienteFornecedor.Id,
                Nome = clienteFornecedor.Nome,
                CpfCnpj = clienteFornecedor.CpfCnpj,
                Email = clienteFornecedor.Email,
                Telefone = clienteFornecedor.Telefone,
                Celular = clienteFornecedor.Celular,
                Endereco = clienteFornecedor.Endereco,
                Tipo = clienteFornecedor.Tipo,
                Status = clienteFornecedor.Status
            };
        }

        public async Task<int> AdicionarAsync(ClienteFornecedorCreateDto dto)
        {
            var clienteFornecedor = new ClienteFornecedor
            {
                Nome = dto.Nome,
                CpfCnpj = dto.CpfCnpj,
                Email = dto.Email,
                Telefone = dto.Telefone,
                Celular = dto.Celular,
                Endereco = dto.Endereco,
                Tipo = dto.Tipo,
                Status = dto.Status
            };

            await _repository.AddAsync(clienteFornecedor);
            return clienteFornecedor.Id;
        }

        public async Task AtualizarAsync(ClienteFornecedorCreateDto dto)
        {
            var clienteFornecedor = await _repository.GetByIdAsync(dto.Id.Value);
            if (clienteFornecedor == null)
            {
                throw new KeyNotFoundException("Cliente/Fornecedor não encontrado.");
            }

            clienteFornecedor.Nome = dto.Nome;
            clienteFornecedor.CpfCnpj = dto.CpfCnpj;
            clienteFornecedor.Email = dto.Email;
            clienteFornecedor.Telefone = dto.Telefone;
            clienteFornecedor.Celular = dto.Celular;
            clienteFornecedor.Endereco = dto.Endereco;
            clienteFornecedor.Tipo = dto.Tipo;
            clienteFornecedor.Status = dto.Status;

            await _repository.UpdateAsync(clienteFornecedor);
        }

        public async Task RemoverAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}