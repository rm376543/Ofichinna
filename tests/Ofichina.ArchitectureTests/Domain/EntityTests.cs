using NetArchTest.Rules;
using Ofichina.Domain.Entities;

namespace Ofichina.ArchitectureTests.Domain;

public class EntityTests
{
    [Fact]
    public void Entidades_Devem_Herdar_De_Entity()
    {
        var result = Types.InAssembly(typeof(Entity).Assembly)
            .That()
            .AreClasses()
            .And()
            .AreNotAbstract()
            .And()
            .ResideInNamespaceContaining("Entities")
            .Should()
            .Inherit(typeof(Entity))
            .GetResult();

        Assert.True(
            result.IsSuccessful,
            "Todos os tipos em 'Entities' devem herdar de Entity.");
    }
}