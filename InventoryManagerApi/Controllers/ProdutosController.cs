using InventoryManagerApi.Data;
using InventoryManagerApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagerApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProdutosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProdutosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutos([FromQuery] string? nome, [FromQuery] bool? status)
        {
            var query = _context.Produtos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(nome))
                query = query.Where(p => p.Nome.Contains(nome));

            if (status.HasValue)
                query = query.Where(p => p.Status == status.Value);

            return await query.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Produto>> GetProduto(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null || !produto.Status) return NotFound();
            return produto;
        }

        [HttpPost]
        public async Task<ActionResult<Produto>> PostProduto(Produto produto)
        {
            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProduto), new { id = produto.Id }, produto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduto(int id, Produto produto)
        {
            if (id != produto.Id) return BadRequest();
            _context.Entry(produto).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduto(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null) return NotFound();
            produto.Status = false;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
