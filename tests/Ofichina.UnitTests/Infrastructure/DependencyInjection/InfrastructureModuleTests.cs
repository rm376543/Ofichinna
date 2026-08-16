using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Ofichina.Application.Abstractions.Authentication.Repository;
using Ofichina.Application.Abstractions.Common;
using Ofichina.Application.Abstractions.Interfaces.Repository;
using Ofichina.Infrastructure.DependencyInjection;
using Ofichina.Infrastructure.Persistence;
using Ofichina.Infrastructure.Repositories;
using Ofichina.Infrastructure.Services;

namespace Ofichina.UnitTests.Infrastructure.DependencyInjection;

public sealed class InfrastructureModuleTests
{
    [Fact]
    public void AddDatabase_Deve_Lancar_Excecao_Quando_ConnectionString_Estiver_Vazia()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        var exception = Assert.Throws<InvalidOperationException>(() => services.AddDatabase(configuration));

        Assert.Contains("DefaultConnection", exception.Message);
    }

    [Fact]
    public void AddRepositories_Deve_Registrar_Servicos_Especificos()
    {
        var services = new ServiceCollection();

        services.AddRepositories();

        Assert.Contains(services, d => d.ServiceType == typeof(IRepository<>));
        Assert.Contains(services, d => d.ServiceType == typeof(IUnitOfWork));
        Assert.Contains(services, d => d.ServiceType == typeof(IUserAuthRepository));
        Assert.Contains(services, d => d.ServiceType == typeof(IPerfilRepository));
        Assert.Contains(services, d => d.ServiceType == typeof(IPermissaoRepository));
        Assert.Contains(services, d => d.ServiceType == typeof(IPerfilPermissaoRepository));
        Assert.Contains(services, d => d.ServiceType == typeof(IPerfilUsuarioRepository));
        Assert.Contains(services, d => d.ServiceType == typeof(IPessoaRepository));
        Assert.Contains(services, d => d.ServiceType == typeof(IVeiculoRepository));
    }

    [Fact]
    public void AddInfrastructureServices_Deve_Registrar_Servicos_De_Infrastructure()
    {
        var services = new ServiceCollection();

        services.AddInfrastructureServices();

        Assert.Contains(services, d => d.ImplementationType == typeof(PerfilAutorizacaoService));
        Assert.Contains(services, d => d.ImplementationType == typeof(MecanicoDisponibilidadeService));
    }

    [Fact]
    public void AddInfrastructure_Deve_Registrar_Modulos_Principais()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Server=(localdb)\\mssqllocaldb;Database=Ofichinna_Testes;Trusted_Connection=True;TrustServerCertificate=True;"
            })
            .Build();

        services.AddInfrastructure(configuration);

        Assert.Contains(services, d => d.ServiceType == typeof(ApplicationDbContext));
        Assert.Contains(services, d => d.ImplementationType == typeof(PerfilAutorizacaoService));
        Assert.Contains(services, d => d.ImplementationType == typeof(MecanicoDisponibilidadeService));
        Assert.Contains(services, d => d.ServiceType == typeof(IHostedService));
    }
}
