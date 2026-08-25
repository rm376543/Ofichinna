using Bogus;
using Ofichina.Domain.Entities;

namespace Ofichina.UnitTests.TestInfrastructure.Fakers;

public class PerfilFaker
{
    private readonly Faker _faker = new();

    public Perfil Criar(Action<Perfil>? customizar = null)
    {
        var nome = _faker.PickRandom(new[] { "Administrador", "Mecanico", "Atendente", "Cliente" });
        var descricao = _faker.Lorem.Sentence();

        var perfil = new Perfil(nome, descricao);

        customizar?.Invoke(perfil);

        return perfil;
    }
}
