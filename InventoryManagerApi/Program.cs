using InventoryManagerApi.Data;
using InventoryManagerApi.Middlewares;
using InventoryManagerApi.Repositories;
using InventoryManagerApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Globalization;

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

// Configuração do banco de dados SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

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
app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    var lastAppliedMigration = dbContext.Database.GetAppliedMigrations().LastOrDefault();
    DateTime? lastAppliedMigrationDate = null;
    if (!string.IsNullOrEmpty(lastAppliedMigration) && lastAppliedMigration.Length >= 14)
    {
        var datePart = lastAppliedMigration.Substring(0, 14);
        if (DateTime.TryParseExact(datePart, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
            lastAppliedMigrationDate = dt;
    }

    // Lê versão e data do arquivo
    string? lastVersion = null;
    DateTime? lastMigrationDateOnFile = null;
    if (File.Exists(versionFile))
    {
        var content = File.ReadAllText(versionFile).Trim();
        var parts = content.Split('|');
        if (parts.Length == 2)
        {
            lastVersion = parts[0];
            if (DateTime.TryParse(parts[1], null, DateTimeStyles.RoundtripKind, out var dt))
                lastMigrationDateOnFile = dt;
        }
    }

    if (lastAppliedMigrationDate != lastMigrationDateOnFile)
    {
        var currentVersion = typeof(Program).Assembly.GetName().Version?.ToString() ?? "0.0.0.0";
        
        var parts = currentVersion.Split('.').Select(int.Parse).ToArray();
        parts[3]++; // incrementa o patch
        var newVersion = $"{parts[0]}.{parts[1]}.{parts[2]}.{parts[3]}";
        currentVersion = newVersion;

        // Verifica se a versão bate com a atual do projeto
        if (lastVersion != currentVersion && app.Environment.IsDevelopment())
        {
            string csprojFile = null;
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir != null)
            {
                var csproj = dir.GetFiles("*.csproj", SearchOption.TopDirectoryOnly).FirstOrDefault();
                if (csproj != null)
                    csprojFile = csproj.FullName;
                dir = dir.Parent;
            };

            if (csprojFile != null)
            {
                var csprojText = File.ReadAllText(csprojFile);
                var versionPattern = @"<Version>(.*?)<\/Version>";
                if (System.Text.RegularExpressions.Regex.IsMatch(csprojText, versionPattern))
                {
                    // Atualiza a versão existente
                    csprojText = System.Text.RegularExpressions.Regex.Replace(
                        csprojText,
                        versionPattern,
                        $"<Version>{currentVersion}</Version>");
                }
                else
                {
                    // Adiciona a tag Version antes do fechamento do primeiro PropertyGroup
                    var propertyGroupPattern = @"(<PropertyGroup[^>]*>)";
                    var match = System.Text.RegularExpressions.Regex.Match(csprojText, propertyGroupPattern);
                    if (match.Success)
                    {
                        var insertIndex = match.Index + match.Length;
                        csprojText = csprojText.Insert(insertIndex, $"\n    <Version>{currentVersion}</Version>");
                    }
                }
                File.WriteAllText(csprojFile, csprojText);
                Console.WriteLine($"Versão do projeto atualizada no csproj: {currentVersion}");
            }
        }

        // Faz o backup do banco
        if (File.Exists(dbPath))
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var backupFile = Path.Combine(backupDirectory, $"inventory_manager_{timestamp}.db");

            File.Copy(dbPath, backupFile, overwrite: true);
            Console.WriteLine($"Backup criado para nova versão {currentVersion}: {backupFile}");

            // Atualiza o arquivo de versão
            File.WriteAllText(versionFile, $"{currentVersion}|{lastAppliedMigrationDate:O}");
        }

        // Remove backups antigos (mantém os 3 mais recentes)
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

    dbContext.Database.Migrate(); // Aplica as migrações do banco de dados
}

app.Run();
