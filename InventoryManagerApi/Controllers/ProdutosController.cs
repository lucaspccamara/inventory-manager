using InventoryManagerApi.Data;
using InventoryManagerApi.Dtos;
using InventoryManagerApi.Models;
using InventoryManagerApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagerApi.Controllers
{
    [ApiController]
    [Route("api/produtos")]
    public class ProdutosController : ControllerBase
    {
        private readonly ProdutoService _service;

        public ProdutosController(ProdutoService service)
        {
            _service = service;
        }

        [HttpPost("lista")]
        public async Task<IActionResult> GetProdutos([FromBody] PagedRequest<ProdutoFilter> request)
        {
            var result = await _service.ListarProdutosAsync(request);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduto(int id)
        {
            var produto = await _service.ObterPorIdAsync(id);
            if (produto == null) return NotFound();
            return Ok(produto);
        }

        [HttpPost]
        public async Task<IActionResult> PostProduto(ProdutoCreateDto produtoCreateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _service.AdicionarAsync(produtoCreateDto);
            return CreatedAtAction(nameof(GetProduto), new { id = id }, new { id = id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduto(int id, ProdutoCreateDto produtoUpdateDto)
        {
            if (id != produtoUpdateDto.Id)
                return BadRequest("ID da URL não corresponde ao ID da entidade.");

            await _service.AtualizarAsync(produtoUpdateDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduto(int id)
        {
            await _service.RemoverAsync(id);
            return NoContent();
        }
    }
}
