using InventoryManagerApi.Dtos;
using InventoryManagerApi.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagerApi.Controllers
{
    [ApiController]
    [Route("api/clientes")]
    public class ClienteFornecedorController : ControllerBase
    {
        private readonly ClienteFornecedorService _service;

        public ClienteFornecedorController(ClienteFornecedorService service)
        {
            _service = service;
        }

        [HttpPost("lista")]
        public async Task<IActionResult> GetClientesFornecedores([FromBody] PagedRequest<ClienteFornecedorFilter> request)
        {
            var response = await _service.ListarClienteFornecedorAsync(request);
            return Ok(response);
        }

        [HttpPost("busca")]
        public async Task<IActionResult> GetClientesFornecedoresSelect([FromBody] PagedRequest<ClienteFornecedorFilter> request)
        {
            var response = await _service.ListarClienteFornecedorSelectAsync(request);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetClienteFornecedor(int id)
        {
            var clienteFornecedor = await _service.ObterPorIdAsync(id);
            if (clienteFornecedor == null) return NotFound();

            return Ok(clienteFornecedor);
        }

        [HttpPost]
        public async Task<IActionResult> PostClienteFornecedor(ClienteFornecedorCreateDto clienteFornecedorCreatedto)
        {
            var id = await _service.AdicionarAsync(clienteFornecedorCreatedto);
            return CreatedAtAction(nameof(GetClienteFornecedor), new { id }, new { id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutClienteFornecedor(int id, ClienteFornecedorCreateDto clienteFornecedorCreatedto)
        {
            if (id != clienteFornecedorCreatedto.Id)
                return BadRequest("ID da URL não corresponde ao ID da entidade.");

            await _service.AtualizarAsync(clienteFornecedorCreatedto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClienteFornecedor(int id)
        {
            await _service.RemoverAsync(id);
            return NoContent();
        }
    }
}
