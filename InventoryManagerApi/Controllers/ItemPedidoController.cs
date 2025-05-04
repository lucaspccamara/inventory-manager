using InventoryManagerApi.Models;
using InventoryManagerApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagerApi.Controllers
{
    [ApiController]
    [Route("api/itens")]
    public class ItemPedidoController : ControllerBase
    {
        private readonly ItemPedidoService _itemPedidoService;

        public ItemPedidoController(ItemPedidoService itemPedidoService)
        {
            _itemPedidoService = itemPedidoService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetItemPedido(int id)
        {
            var itemPedido = await _itemPedidoService.ObterPorIdAsync(id);
            if (itemPedido == null) return NotFound();
            return Ok(itemPedido);
        }

        [HttpPost]
        public async Task<IActionResult> PostItemPedido(ItemPedido itemPedido)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var itemPedidoId = await _itemPedidoService.AdicionarAsync(itemPedido);
            return CreatedAtAction(nameof(GetItemPedido), new { id = itemPedidoId }, new { id = itemPedidoId });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutItemPedido(int id, ItemPedido itemPedido)
        {
            if (id != itemPedido.Id)
                return BadRequest("ID da URL não corresponde ao ID da entidade.");

            await _itemPedidoService.AtualizarAsync(itemPedido);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItemPedido(int id)
        {
            await _itemPedidoService.RemoverAsync(id);
            return NoContent();
        }
    }
}
