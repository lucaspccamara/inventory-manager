using InventoryManagerApi.Dtos;
using InventoryManagerApi.Models;
using InventoryManagerApi.Services;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> GetUnidades([FromBody] PagedRequest<UnidadeMedidaFilter> request)
        {
            var result = await _service.ListarUnidadesAsync(request);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUnidade(int id)
        {
            var unidadeDto = await _service.ObterPorIdAsync(id);
            if (unidadeDto == null)
                return NotFound();
            return Ok(unidadeDto);
        }

        [HttpPost]
        public async Task<IActionResult> PostUnidade([FromBody] UnidadeMedidaDto unidadeDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _service.AdicionarAsync(unidadeDto);
            return CreatedAtAction(nameof(GetUnidade), new { id }, new { id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUnidade(int id, [FromBody] UnidadeMedidaDto unidadeDto)
        {
            if (id != unidadeDto.Id)
                return BadRequest("ID da URL não corresponde ao ID da entidade.");

            await _service.AtualizarAsync(unidadeDto);
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
