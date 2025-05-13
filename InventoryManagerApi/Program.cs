using InventoryManagerApi.Data;
using InventoryManagerApi.Repositories;
using InventoryManagerApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using System;

var builder = WebApplication.CreateBuilder(args);

// Define caminho seguro para o banco SQLite
var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
var dbDirectory = Path.Combine(localAppData, "InventoryManager");
var dbPath = Path.Combine(dbDirectory, "inventory_manager.db");
var backupDirectory = Path.Combine(dbDirectory, "backups");
var versionFile = Path.Combine(dbDirectory, "last_version.txt");

// Garante que as pastas existem
Directory.CreateDirectory(dbDirectory);
Directory.CreateDirectory(backupDirectory);

// Configura��o do banco de dados SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

// Registro dos servi�os e reposit�rios
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

    // Obter vers�o atual do execut�vel
    var currentVersion = typeof(Program).Assembly.GetName().Version?.ToString() ?? "0.0.0.0";

    // Obter vers�o anterior salva
    string? lastVersion = null;
    if (File.Exists(versionFile))
    {
        lastVersion = File.ReadAllText(versionFile).Trim();
    }

    // S� faz backup se a vers�o mudou
    if (lastVersion != currentVersion)
    {
        if (File.Exists(dbPath))
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var backupFile = Path.Combine(backupDirectory, $"inventory_manager_{timestamp}.db");

            File.Copy(dbPath, backupFile, overwrite: true);
            Console.WriteLine($"Backup criado para nova vers�o {currentVersion}: {backupFile}");

            // Atualiza o arquivo de vers�o
            File.WriteAllText(versionFile, currentVersion);
        }

        // Remove backups antigos (mant�m os 3 mais recentes)
        var backups = new DirectoryInfo(backupDirectory)
            .GetFiles("inventory_manager_*.db")
            .OrderByDescending(f => f.CreationTime)
            .Skip(3);

        foreach (var oldBackup in backups)
        {
            try
            {
                oldBackup.Delete();
                Console.WriteLine($"Backup antigo removido: {oldBackup.Name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao remover backup antigo: {ex.Message}");
            }
        }
    }

    dbContext.Database.Migrate(); // Aplica as migra��es do banco de dados
}

app.Run();
