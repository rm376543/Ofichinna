using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Ofichina.Infrastructure.Persistence;
using Testcontainers.MsSql;

namespace Ofichina.IntegrationTests;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _container = new MsSqlBuilder(
        "mcr.microsoft.com/mssql/server:2022-latest")
        .WithPassword("P@ssw0rd2024!Ofichina")
        .Build();

    private string? _connectionString;

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        var builder = new SqlConnectionStringBuilder(_container.GetConnectionString())
        {
            InitialCatalog = "Ofichina.IntegrationTests"
        };

        _connectionString = builder.ConnectionString;

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(_connectionString)
            .Options;

        await using var context = new ApplicationDbContext(options);
        await context.Database.MigrateAsync();
    }

#pragma warning disable CS0114
    public async Task DisposeAsync()
#pragma warning restore CS0114
    {
        Dispose();
        await _container.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                throw new InvalidOperationException("A connection string de teste ainda não foi inicializada.");
            }

            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _connectionString
            });
        });
    }
}