using InventoryManagerApi.Data;
using InventoryManagerApi.Repositories;
using InventoryManagerApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using System;

var builder = WebApplication.CreateBuilder(args);

// Configuração do banco de dados SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=inventory_manager.db"));

// Registro dos serviços e repositórios
builder.Services.AddScoped<ClienteFornecedorService>();
builder.Services.AddScoped<ItemPedidoService>();
builder.Services.AddScoped<MovimentacaoEstoqueService>();
builder.Services.AddScoped<PedidoService>();
builder.Services.AddScoped<ProdutoService>();
builder.Services.AddScoped<UnidadeMedidaService>();

builder.Services.AddScoped<ClienteFornecedorRepository>();
builder.Services.AddScoped<ItemPedidoRepository>();
builder.Services.AddScoped<MovimentacaoEstoqueRepository>();
builder.Services.AddScoped<PedidoRepository>();
builder.Services.AddScoped<ProdutoRepository>();
builder.Services.AddScoped<ProdutoUnidadeVendaRepository>();
builder.Services.AddScoped<UnidadeMedidaRepository>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate(); // Aplica as migrações do banco de dados
}

app.Run();
