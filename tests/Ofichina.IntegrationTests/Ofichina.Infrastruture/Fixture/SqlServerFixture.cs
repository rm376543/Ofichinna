using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Ofichina.Infrastructure.Persistence;
using Testcontainers.MsSql;

namespace Ofichina.IntegrationTests.Infrastructure;

public sealed class SqlServerFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _container = new MsSqlBuilder(
            "mcr.microsoft.com/mssql/server:2022-latest")
        .WithPassword("P@ssw0rd2024!Ofichina")
        .Build();

    private string _connectionString = string.Empty;

    public async Task InitializeAsync()
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

    public ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(_connectionString)
            .Options;

        return new ApplicationDbContext(options);
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}