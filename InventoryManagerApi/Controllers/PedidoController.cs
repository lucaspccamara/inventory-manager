using InventoryManagerApi.Dtos;
using InventoryManagerApi.Models;
using InventoryManagerApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagerApi.Controllers
{
    [ApiController]
    [Route("api/pedidos")]
    public class PedidoController : ControllerBase
    {
        private readonly PedidoService _pedidoService;

        public PedidoController(PedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        [HttpPost("lista")]
        public async Task<IActionResult> GetPedidos([FromBody] PagedRequest<PedidoFilter> request)
        {
            var result = await _pedidoService.ListarPedidosAsync(request);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPedido(int id)
        {
            var pedido = await _pedidoService.ObterPorIdAsync(id);
            if (pedido == null) return NotFound();
            return Ok(pedido);
        }

        [HttpPost]
        public async Task<IActionResult> PostPedido(PedidoCreateDto pedidoCreateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _pedidoService.AdicionarAsync(pedidoCreateDto);
            return CreatedAtAction(nameof(GetPedido), new { id }, new { id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPedido(int id, PedidoUpdateDto pedidoUpdateDto)
        {
            if (id != pedidoUpdateDto.Id)
                return BadRequest("ID da URL não corresponde ao ID da entidade.");

            await _pedidoService.AtualizarAsync(pedidoUpdateDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelarPedido(int id)
        {
            await _pedidoService.CancelarAsync(id);
            return NoContent();
        }

        [HttpPut("{id}/restaurar")]
        public async Task<IActionResult> RestaurarPedido(int id)
        {
            await _pedidoService.RestaurarAsync(id);
            return NoContent();

        }

        [HttpGet("{id}/pdf")]
        public async Task<IActionResult> GerarPdf(int id)
        {
            var pedidoDto = await _pedidoService.ObterPorIdAsync(id);
            if (pedidoDto == null)
                return NotFound();

            var pdfBytes = _pedidoService.GerarPdf(pedidoDto);

            return File(pdfBytes, "application/pdf", $"pedido_{id}.pdf");
        }
    }
}
