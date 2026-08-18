using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ofichina.Bootstrap;

namespace Ofichina.UnitTests.Bootstrap.DependencyInjection;

public sealed class DependencyInjectionTests
{
    private static IConfiguration CriarConfiguracaoValida()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "chave-de-teste-com-tamanho-suficiente-para-hmac-sha256",
                ["ConnectionStrings:DefaultConnection"] = "Server=(localdb)\\MSSQLLocalDB;Database=OfichinaTestes;Trusted_Connection=True;TrustServerCertificate=True;",
            })
            .Build();
    }

    [Fact]
    public void AddBootstrapMiddleware_QuandoChamado_DeveRetornarAMesmaInstanciaDeIServiceCollection()
    {
        var services = new ServiceCollection();
        var configuration = CriarConfiguracaoValida();

        var result = services.AddBootstrapMiddleware(configuration);

        Assert.Same(services, result);
    }

    [Fact]
    public void AddBootstrapMiddleware_QuandoChamado_DeveRegistrarServicosDeTodosOsModulos()
    {
        var services = new ServiceCollection();
        var configuration = CriarConfiguracaoValida();

        services.AddBootstrapMiddleware(configuration);

        Assert.NotEmpty(services);
    }

    [Fact]
    public void AddBootstrapMiddleware_QuandoConfiguracaoEhValida_NaoDeveLancarExcecao()
    {
        var services = new ServiceCollection();
        var configuration = CriarConfiguracaoValida();

        var exception = Record.Exception(() => services.AddBootstrapMiddleware(configuration));

        Assert.Null(exception);
    }

    [Fact]
    public void AddBootstrapMiddleware_QuandoJwtKeyNaoEstaConfigurada_DeveLancarInvalidOperationException()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        var exception = Record.Exception(() => services.AddBootstrapMiddleware(configuration));

        Assert.IsType<InvalidOperationException>(exception);
    }
}