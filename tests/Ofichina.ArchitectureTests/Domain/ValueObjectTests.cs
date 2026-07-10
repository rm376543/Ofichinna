using System.Reflection;
using Ofichina.Domain.ValueObjects;

namespace Ofichina.ArchitectureTests.Domain;

public class ValueObjectTests
{
    [Fact]
    public void ValueObjects_Nao_Devem_Possuir_Id()
    {
        var invalidTypes = typeof(ValueObject).Assembly
            .GetTypes()
            .Where(type =>
                type is { IsClass: true, IsAbstract: false } &&
                type.Namespace?.Contains("ValueObjects", StringComparison.Ordinal) == true &&
                type != typeof(ValueObject) &&
                typeof(ValueObject).IsAssignableFrom(type))
            .Where(type => type.GetProperty("Id", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly) is not null)
            .Select(type => type.FullName)
            .ToArray();

        Assert.True(
            invalidTypes.Length == 0,
            $"Os ValueObjects abaixo não devem possuir propriedade 'Id': {string.Join(", ", invalidTypes)}");
    }
}