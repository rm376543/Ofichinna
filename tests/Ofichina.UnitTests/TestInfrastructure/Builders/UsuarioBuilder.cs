using Ofichina.Domain.Entities;
using Ofichina.Domain.ValueObjects;

namespace Ofichina.UnitTests.TestInfrastructure.Builders;

public class UsuarioBuilder
{
    private Usuario _usuario;

    public UsuarioBuilder()
    {
        _usuario = TestDataFactory.Usuarios.Criar();
    }

    public UsuarioBuilder ComId(Guid id)
    {
        ReflectionHelpers.DefinirId(_usuario, id);
        return this;
    }

    public UsuarioBuilder ComEmail(string email)
    {
        _usuario.AlterarEmail(new Email(email));
        return this;
    }

    public UsuarioBuilder ComSenhaHash(string senhaHash)
    {
        _usuario.AlterarSenha(senhaHash);
        return this;
    }

    public UsuarioBuilder AdicionarPerfil(UsuarioPerfil usuarioPerfil)
    {
        _usuario.AdicionarPerfil(usuarioPerfil);
        return this;
    }

    public Usuario Build() => _usuario;
}
