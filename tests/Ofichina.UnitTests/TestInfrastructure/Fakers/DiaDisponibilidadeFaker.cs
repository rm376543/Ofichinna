using Bogus;
using Ofichina.Domain.Entities;

namespace Ofichina.UnitTests.TestInfrastructure.Fakers;

public class DiaDisponibilidadeFaker
{
    private readonly Faker _faker = new();

    public DiaDisponibilidade Criar(DateOnly? data = null, Action<DiaDisponibilidade>? customizar = null)
    {
        var d = data ?? DateOnly.FromDateTime(DateTime.UtcNow.AddDays(_faker.Random.Int(1, 30)));
        var dia = new DiaDisponibilidade(d);

        customizar?.Invoke(dia);

        return dia;
    }
}
