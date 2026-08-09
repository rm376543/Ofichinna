using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ofichina.Infrastructure.Persistence;

namespace Ofichina.Infrastructure.DependencyInjection;

public sealed class DatabaseInitializationHostedService : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHostEnvironment _environment;
    private readonly ILogger<DatabaseInitializationHostedService> _logger;

    public DatabaseInitializationHostedService(
        IServiceScopeFactory scopeFactory,
        IHostEnvironment environment,
        ILogger<DatabaseInitializationHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _environment = environment;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_environment.IsDevelopment())
        {
            _logger.LogInformation(
                "Inicialização do banco ignorada no ambiente {Environment}.",
                _environment.EnvironmentName);

            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        _logger.LogInformation("Aplicando migrations e executando seed do banco.");
        await context.Database.MigrateAsync(cancellationToken);
        await DatabaseSeeder.SeedAsync(context);
        _logger.LogInformation("Inicialização do banco concluída.");
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}