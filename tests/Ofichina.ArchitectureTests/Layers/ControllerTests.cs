using NetArchTest.Rules;
using Ofichina.Api.Controllers.Autenticacao;
using Ofichina.Api.Controllers.Perfis;

namespace Ofichina.ArchitectureTests.Layers;

public class ControllerTests
{
    [Fact]
    public void Controllers_Devem_Terminar_Com_Controller()
    {
        var result = Types.InAssembly(typeof(AuthController).Assembly)
            .That()
            .ResideInNamespace("Ofichina.Api.Controllers")
            .Should()
            .HaveNameEndingWith("Controller")
            .GetResult();

        Assert.True(
            result.IsSuccessful,
            "Todos os tipos em 'Ofichina.Api.Controllers' devem terminar com 'Controller'.");
    }
}