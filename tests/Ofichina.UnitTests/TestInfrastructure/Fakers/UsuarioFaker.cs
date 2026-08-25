using Bogus;
using Ofichina.Domain.Entities;
using Ofichina.Domain.ValueObjects;

namespace Ofichina.UnitTests.TestInfrastructure.Fakers;

public class UsuarioFaker
{
    private readonly Faker _faker = new();

    public Usuario Criar(Action<Usuario>? customizar = null)
    {
        var email = new Email(_faker.Internet.Email());
        var senha = _faker.Internet.Password(8);

        var usuario = new Usuario(email, senha);

        customizar?.Invoke(usuario);

        return usuario;
    }
}
