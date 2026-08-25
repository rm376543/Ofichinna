using Bogus;
using Ofichina.Domain.Entities;
using Ofichina.Domain.ValueObjects;

namespace Ofichina.UnitTests.TestInfrastructure.Fakers;

public class VeiculoFaker
{
    private readonly Faker _faker = new();

    public Veiculo Criar(Guid? pessoaId = null, Action<Veiculo>? customizar = null)
    {
        var pid = pessoaId ?? _faker.Random.Guid();
        var placa = new Placa(FakerHelpers.GerarPlaca());
        var marca = _faker.Vehicle.Manufacturer();
        var modelo = _faker.Vehicle.Model();
        var ano = _faker.Random.Int(1900, DateTime.Now.Year);
        var cor = _faker.Commerce.Color();
        var hodometro = new Hodometro(FakerHelpers.GerarHodometro());

        var veiculo = new Veiculo(pid, placa, marca, modelo, ano, cor, hodometro);

        customizar?.Invoke(veiculo);

        return veiculo;
    }
}
