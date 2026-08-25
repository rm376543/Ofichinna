using Bogus;
using Ofichina.Domain.Entities;

namespace Ofichina.UnitTests.TestInfrastructure.Fakers;

public class HorarioDisponibilidadeFaker
{
    private readonly Faker _faker = new();

    public HorarioDisponibilidade Criar(TimeOnly? hora = null, Action<HorarioDisponibilidade>? customizar = null)
    {
        var h = hora ?? TimeOnly.FromTimeSpan(TimeSpan.FromHours(_faker.Random.Int(8, 17)));
        var horario = new HorarioDisponibilidade(h);

        customizar?.Invoke(horario);

        return horario;
    }
}
