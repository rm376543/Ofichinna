using NetArchTest.Rules;
using Ofichina.Application.Validators;
using Ofichina.Authentication.Validators;

namespace Ofichina.ArchitectureTests.Naming;

public class ValidatorTests
{
    [Fact]
    public void Validators_Devem_Terminar_Com_Validator()
    {
        AssertAssemblyValidators(typeof(CreateExemploRequestValidator).Assembly);
        AssertAssemblyValidators(typeof(AutenticacaoRequestValidator).Assembly);
    }

    private static void AssertAssemblyValidators(System.Reflection.Assembly assembly)
    {
        var result = Types.InAssembly(assembly)
            .That()
            .ResideInNamespaceContaining("Validators")
            .Should()
            .HaveNameEndingWith("Validator")
            .GetResult();

        Assert.True(
            result.IsSuccessful,
            $"Todos os tipos em '{assembly.GetName().Name}' dentro de namespaces contendo 'Validators' devem terminar com 'Validator'.");
    }
}