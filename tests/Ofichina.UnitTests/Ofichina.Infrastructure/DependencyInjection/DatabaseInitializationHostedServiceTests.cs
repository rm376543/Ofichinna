using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Ofichina.Infrastructure.DependencyInjection;
using Ofichina.Infrastructure.Persistence;

namespace Ofichina.UnitTests.Infrastructure.DependencyInjection;

public sealed class DatabaseInitializationHostedServiceTests
{
    [Fact]
    public void DatabaseInitializationHostedService_QuandoDependenciasSaoValidas_DeveCriarInstancia()
    {
        var scopeFactory = new Mock<IServiceScopeFactory>();
        var environment = new Mock<IHostEnvironment>();
        var logger = new Mock<ILogger<DatabaseInitializationHostedService>>();

        var service = new DatabaseInitializationHostedService(
            scopeFactory.Object,
            environment.Object,
            logger.Object);

        Assert.NotNull(service);
    }

    [Fact]
    public async Task StartAsync_QuandoAmbienteNaoEhDesenvolvimento_DeveIgnorarInicializacao()
    {
        var scopeFactory = new Mock<IServiceScopeFactory>(MockBehavior.Strict);
        var environment = new Mock<IHostEnvironment>();
        var logger = new Mock<ILogger<DatabaseInitializationHostedService>>();

        environment.SetupGet(x => x.EnvironmentName).Returns("Production");

        var service = new DatabaseInitializationHostedService(
            scopeFactory.Object,
            environment.Object,
            logger.Object);

        await service.StartAsync(CancellationToken.None);

        scopeFactory.Verify(x => x.CreateScope(), Times.Never);
        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString() == "Inicialização do banco ignorada no ambiente Production."),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task StartAsync_QuandoAmbienteEhDesenvolvimento_DeveAplicarMigracoesEExecutarSeed()
    {
        var databaseName = $"Ofichina_DatabaseInitializationHostedServiceTests_{Guid.NewGuid():N}";
        var connectionString = $"Server=(localdb)\\MSSQLLocalDB;Database={databaseName};Trusted_Connection=True;TrustServerCertificate=True;";

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(connectionString)
            .ConfigureWarnings(x => x.Ignore(RelationalEventId.PendingModelChangesWarning))
            .Options;

        await using var context = new ApplicationDbContext(options);

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider
            .Setup(x => x.GetService(typeof(ApplicationDbContext)))
            .Returns(context);

        var scope = new Mock<IServiceScope>();
        scope.SetupGet(x => x.ServiceProvider).Returns(serviceProvider.Object);
        scope.Setup(x => x.Dispose());

        var scopeFactory = new Mock<IServiceScopeFactory>();
        scopeFactory.Setup(x => x.CreateScope()).Returns(scope.Object);

        var environment = new Mock<IHostEnvironment>();
        environment.SetupGet(x => x.EnvironmentName).Returns("Development");

        var logger = new Mock<ILogger<DatabaseInitializationHostedService>>();

        var service = new DatabaseInitializationHostedService(
            scopeFactory.Object,
            environment.Object,
            logger.Object);

        await service.StartAsync(CancellationToken.None);

        scopeFactory.Verify(x => x.CreateScope(), Times.Once);
        scope.Verify(x => x.Dispose(), Times.Once);
        Assert.True(await context.Perfis.AnyAsync());
        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) =>
                    v.ToString() == "Aplicando migrations e executando seed do banco." ||
                    v.ToString() == "Inicialização do banco concluída."),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Exactly(2));
    }

    [Fact]
    public async Task StopAsync_QuandoChamado_DeveRetornarTarefaConcluida()
    {
        var scopeFactory = new Mock<IServiceScopeFactory>();
        var environment = new Mock<IHostEnvironment>();
        var logger = new Mock<ILogger<DatabaseInitializationHostedService>>();

        var service = new DatabaseInitializationHostedService(
            scopeFactory.Object,
            environment.Object,
            logger.Object);

        var result = service.StopAsync(CancellationToken.None);

        await result;
        Assert.True(result.IsCompletedSuccessfully);
    }
}
