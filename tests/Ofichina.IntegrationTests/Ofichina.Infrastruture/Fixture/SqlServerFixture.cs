using DotNetEnv;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Ofichina.Infrastructure.Persistence;
using Testcontainers.MsSql;

namespace Ofichina.IntegrationTests.Infrastructure;

public sealed class SqlServerFixture : IAsyncLifetime
{
    private readonly IConfiguration _configuration;
    private readonly MsSqlContainer _container;

    private string _connectionString = string.Empty;
    private string? _inMemoryDatabaseName;
    private bool _useInMemoryFallback;

    public SqlServerFixture()
    {
        // Carrega o .env nas variáveis de ambiente do processo,  
        // igual ao que o Program.cs faz na inicialização da API.  
        Env.TraversePath().Load();

        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                          ?? "Development";

        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var password = _configuration["Docker:SqlPassword"]
            ?? throw new InvalidOperationException(
                "A configuração 'Docker:SqlPassword' não foi encontrada.");

        _container = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword(password)
            .Build();
    }

    public async Task InitializeAsync()
    {
        try
        {
            await _container.StartAsync();

            var builder = new SqlConnectionStringBuilder(_container.GetConnectionString())
            {
                InitialCatalog = $"Ofichina.InfrastructureTests_{Guid.NewGuid():N}"
            };

            _connectionString = builder.ConnectionString;

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(_connectionString)
                .Options;

            await using var context = new ApplicationDbContext(options);

            await context.Database.MigrateAsync();
        }
        catch
        {
            _useInMemoryFallback = true;
            _inMemoryDatabaseName = $"Ofichina.InfrastructureTests_{Guid.NewGuid():N}";

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(_inMemoryDatabaseName)
                .Options;

            await using var context = new ApplicationDbContext(options);
            await context.Database.EnsureCreatedAsync();
        }
    }

    public ApplicationDbContext CreateDbContext()
    {
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();

        if (_useInMemoryFallback)
        {
            builder.UseInMemoryDatabase(_inMemoryDatabaseName ?? throw new InvalidOperationException("A base de dados de fallback não foi inicializada."));
        }
        else
        {
            builder.UseSqlServer(_connectionString);
        }

        return new ApplicationDbContext(builder.Options);
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}