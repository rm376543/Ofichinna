using NetArchTest.Rules;
using Ofichina.Application.UseCases.Autenticacao.Handlers;

namespace Ofichina.ArchitectureTests.Naming;

public class HandlerTests
{
    [Fact]
    public void Handlers_Devem_Terminar_Com_Handler()
    {
        AssertAssemblyHandlers(typeof(AutenticarCommandHandler).Assembly);
    }

    private static void AssertAssemblyHandlers(System.Reflection.Assembly assembly)
    {
        var result = Types.InAssembly(assembly)
            .That()
            .ResideInNamespaceContaining("Handlers")
            .Should()
            .HaveNameEndingWith("Handler")
            .GetResult();

        Assert.True(
            result.IsSuccessful,
            $"Todos os tipos em '{assembly.GetName().Name}' dentro de namespaces contendo 'Handlers' devem terminar com 'Handler'.");
    }
}