using Bogus;
using Ofichina.Domain.Entities;

namespace Ofichina.UnitTests.TestInfrastructure.Fakers;

public class PerfilPermissaoFaker
{
    private readonly Faker _faker = new();

    public PerfilPermissao Criar(Guid? perfilId = null, Guid? permissaoId = null, Action<PerfilPermissao>? customizar = null)
    {
        var pid = perfilId ?? Guid.NewGuid();
        var perId = permissaoId ?? Guid.NewGuid();

        var pp = new PerfilPermissao(pid, perId);

        customizar?.Invoke(pp);

        return pp;
    }
}
