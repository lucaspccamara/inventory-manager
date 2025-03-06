using InventoryManagerApi.Data;
using InventoryManagerApi.Dtos;
using InventoryManagerApi.Models;
using InventoryManagerApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagerApi.Controllers
{
    [ApiController]
    [Route("api/unidades")]
    public class UnidadeMedidaController : ControllerBase
    {
        private readonly UnidadeMedidaService _service;

        public UnidadeMedidaController(UnidadeMedidaService service)
        {
            _service = service;
        }

        [HttpPost("lista")]
        public async Task<IActionResult> GetUnidades([FromBody] PagedRequest<UnidadeFilter> request)
        {
            var result = await _service.ListarUnidadesAsync(request);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUnidade(int id)
        {
            var unidade = await _service.ObterPorIdAsync(id);
            if (unidade == null)
                return NotFound();
            return Ok(unidade);
        }

        [HttpPost]
        public async Task<IActionResult> PostUnidade([FromBody] UnidadeMedida unidade)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _service.AdicionarAsync(unidade);
            return CreatedAtAction(nameof(GetUnidade), new { id = unidade.Id }, unidade);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUnidade(int id, [FromBody] UnidadeMedida unidade)
        {
            if (id != unidade.Id)
                return BadRequest("ID da URL não corresponde ao ID da entidade.");

            await _service.AtualizarAsync(unidade);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUnidade(int id)
        {
            await _service.RemoverAsync(id);
            return NoContent();
        }
    }
}
