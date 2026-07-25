using System.Reflection;
using Ofichina.Application.Abstractions.Authentication;
using Ofichina.Application.Abstractions.Common;

namespace Ofichina.ArchitectureTests.Naming;

public class RepositoryTests
{
    [Fact]
    public void Repositories_Devem_Ser_Interfaces()
    {
        AssertRepositoryInterfaces(typeof(IRepository<>).Assembly);
        AssertRepositoryInterfaces(typeof(IUsuarioAutenticacaoRepository).Assembly);
    }

    private static void AssertRepositoryInterfaces(Assembly assembly)
    {
        var invalidTypes = assembly.GetTypes()
            .Where(type => type.Name.EndsWith("Repository", StringComparison.Ordinal) && !type.IsInterface)
            .Select(type => type.FullName)
            .ToArray();

        Assert.True(
            invalidTypes.Length == 0,
            $"Os tipos abaixo terminam com 'Repository' mas não são interfaces em '{assembly.GetName().Name}': {string.Join(", ", invalidTypes)}");
    }
}