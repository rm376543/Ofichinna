using Bogus;
using Ofichina.Domain.Entities;

namespace Ofichina.UnitTests.TestInfrastructure.Fakers;

public class UsuarioPerfilFaker
{
    private readonly Faker _faker = new();

    public UsuarioPerfil Criar(Guid? usuarioId = null, Guid? perfilId = null, Action<UsuarioPerfil>? customizar = null)
    {
        var uid = usuarioId ?? Guid.NewGuid();
        var pid = perfilId ?? Guid.NewGuid();

        var up = new UsuarioPerfil(uid, pid);

        customizar?.Invoke(up);

        return up;
    }
}
