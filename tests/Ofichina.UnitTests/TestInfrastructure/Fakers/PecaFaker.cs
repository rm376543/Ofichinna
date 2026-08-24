using Bogus;
using Ofichina.Domain.Entities;

namespace Ofichina.UnitTests.TestInfrastructure.Fakers;

public class PecaFaker
{
    private readonly Faker _faker = new();

    public Peca Criar(Action<Peca>? customizar = null)
    {
        var nome = _faker.Commerce.ProductName();
        var descricao = _faker.Commerce.Product();
        var codigo = FakerHelpers.GerarCodigoPeca();
        var valor = Math.Round(_faker.Random.Decimal(5m, 500m), 2);
        var quantidade = _faker.Random.Int(0, 100);

        var peca = new Peca(nome, descricao, codigo, valor, quantidade);

        customizar?.Invoke(peca);

        return peca;
    }
}
