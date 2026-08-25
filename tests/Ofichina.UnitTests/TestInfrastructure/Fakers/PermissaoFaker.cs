using Bogus;
using Ofichina.Domain.Entities;

namespace Ofichina.UnitTests.TestInfrastructure.Fakers;

public class PermissaoFaker
{
    private readonly Faker _faker = new();

    public Permissao Criar(Action<Permissao>? customizar = null)
    {
        var codigo = $"PERM_{_faker.Random.Guid():N}";
        var descricao = _faker.Lorem.Sentence();

        var p = new Permissao(codigo, descricao);

        customizar?.Invoke(p);

        return p;
    }
}
