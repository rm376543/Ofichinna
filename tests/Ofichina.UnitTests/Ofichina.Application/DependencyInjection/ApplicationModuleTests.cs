using Microsoft.Extensions.DependencyInjection;
using Ofichina.Application.DependencyInjection;

namespace Ofichina.UnitTests.Application.DependencyInjection;

public sealed class ApplicationModuleTests
{
    [Fact]
    public void AddApplication_QuandoChamado_DeveRetornarAMesmaInstanciaDeIServiceCollection()
    {
        var services = new ServiceCollection();

        var result = services.AddApplication();

        Assert.Same(services, result);
    }

    [Fact]
    public void AddApplication_QuandoChamado_DeveRegistrarServicosDeTodosOsModulos()
    {
        var services = new ServiceCollection();

        services.AddApplication();

        Assert.NotEmpty(services);
    }

    [Fact]
    public void AddApplication_QuandoChamado_NaoDeveLancarExcecao()
    {
        var services = new ServiceCollection();

        var exception = Record.Exception(() => services.AddApplication());

        Assert.Null(exception);
    }
}