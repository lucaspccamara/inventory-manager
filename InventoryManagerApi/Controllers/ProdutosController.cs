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

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Produto>>> GetProdutos([FromQuery] string? nome, [FromQuery] bool? status)
        //{
        //    var query = _context.Produtos.AsQueryable();

        //    if (!string.IsNullOrWhiteSpace(nome))
        //        query = query.Where(p => p.Nome.Contains(nome));

        //    if (status.HasValue)
        //        query = query.Where(p => p.Status == status.Value);

        //    return await query.ToListAsync();
        //}

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduto(int id)
        {
            var produto = await _service.ObterPorIdAsync(id);
            if (produto == null || !produto.Status) return NotFound();
            return Ok(produto);
        }

        [HttpPost]
        public async Task<IActionResult> PostProduto(ProdutoDto produtoDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _service.AdicionarAsync(produtoDto);
            return CreatedAtAction(nameof(GetProduto), new { id = produtoDto.Id }, new { id = produtoDto.Id });
        }

        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutProduto(int id, Produto produto)
        //{
        //    if (id != produto.Id) return BadRequest();
        //    _context.Entry(produto).State = EntityState.Modified;
        //    await _context.SaveChangesAsync();
        //    return NoContent();
        //}

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteProduto(int id)
        //{
        //    var produto = await _context.Produtos.FindAsync(id);
        //    if (produto == null) return NotFound();
        //    produto.Status = false;
        //    await _context.SaveChangesAsync();
        //    return NoContent();
        //}
    }
}
