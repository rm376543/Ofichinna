using Bogus;
using Ofichina.Domain.Entities;

namespace Ofichina.UnitTests.TestInfrastructure.Fakers;

public class DiaHorarioDisponibilidadeFaker
{
    private readonly Faker _faker = new();

    public DiaHorarioDisponibilidade Criar(Guid? diaDisponibilidadeId = null, Guid? horarioDisponibilidadeId = null, Action<DiaHorarioDisponibilidade>? customizar = null)
    {
        var diaId = diaDisponibilidadeId ?? _faker.Random.Guid();
        var horarioId = horarioDisponibilidadeId ?? _faker.Random.Guid();

        var dh = new DiaHorarioDisponibilidade(diaId, horarioId);

        customizar?.Invoke(dh);

        return dh;
    }
}
