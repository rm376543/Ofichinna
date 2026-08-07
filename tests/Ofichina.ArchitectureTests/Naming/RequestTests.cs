using NetArchTest.Rules;
using Ofichina.Contracts.Common;

namespace Ofichina.ArchitectureTests.Naming;

public class RequestTests
{
    [Fact]
    public void Requests_Devem_Terminar_Com_Request()
    {
        var result = Types.InAssembly(typeof(BaseRequest).Assembly)
            .That()
            .ResideInNamespaceContaining("Requests")
            .Should()
            .HaveNameEndingWith("Request")
            .GetResult();

        Assert.True(
            result.IsSuccessful,
            "Todos os tipos em namespaces contendo 'Requests' devem terminar com 'Request'.");
    }
}