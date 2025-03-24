using InventoryManagerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagerApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Produto> Produtos { get; set; }
        public DbSet<UnidadeMedida> UnidadesMedida { get; set; }
        public DbSet<ProdutoUnidadeVenda> ProdutosUnidadeVenda { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<ItemPedido> ItensPedido { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UnidadeMedida>()
            .HasMany(u => u.ProdutosCompra)
            .WithOne(p => p.UnidadeMedidaCompra)
            .HasForeignKey(p => p.UnidadeMedidaCompraId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UnidadeMedida>()
                .HasMany(u => u.ProdutosMenorUnidade)
                .WithOne(p => p.MenorUnidade)
                .HasForeignKey(p => p.MenorUnidadeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProdutoUnidadeVenda>()
                .HasOne(puv => puv.Produto)
                .WithMany(p => p.UnidadesVenda)
                .HasForeignKey(puv => puv.ProdutoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProdutoUnidadeVenda>()
                .HasOne(puv => puv.UnidadeMedida)
                .WithMany(u => u.ProdutosVenda)
                .HasForeignKey(puv => puv.UnidadeMedidaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProdutoUnidadeVenda>()
                .HasOne(puv => puv.MenorUnidade)
                .WithMany()
                .HasForeignKey(puv => puv.MenorUnidadeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ItemPedido>()
                .HasOne(i => i.Pedido)
                .WithMany(p => p.Itens)
                .HasForeignKey(i => i.PedidoId);

            modelBuilder.Entity<ItemPedido>()
                .HasOne(i => i.Produto)
                .WithMany()
                .HasForeignKey(i => i.ProdutoId);
        }
    }
}
