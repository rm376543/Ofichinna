using Bogus;
using Ofichina.Domain.Entities;

namespace Ofichina.UnitTests.TestInfrastructure.Fakers;

public class ServicoFaker
{
    private readonly Faker _faker = new();

    public Servico Criar(Action<Servico>? customizar = null)
    {
        var nome = _faker.Commerce.Department() + " " + _faker.Hacker.Verb();
        var descricao = _faker.Commerce.ProductDescription();
        var valor = Math.Round(_faker.Random.Decimal(10m, 1000m), 2);

        var servico = new Servico(nome, descricao, valor);

        customizar?.Invoke(servico);

        return servico;
    }
}
