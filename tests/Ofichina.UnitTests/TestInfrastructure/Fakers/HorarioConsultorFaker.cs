using Bogus;
using Ofichina.Domain.Entities;

namespace Ofichina.UnitTests.TestInfrastructure.Fakers;

public class HorarioConsultorFaker
{
    private readonly Faker _faker = new();

    public HorarioConsultor Criar(Guid? horarioDisponibilidadeId = null, Guid? pessoaId = null, Action<HorarioConsultor>? customizar = null)
    {
        var hid = horarioDisponibilidadeId ?? _faker.Random.Guid();
        var pid = pessoaId ?? _faker.Random.Guid();

        var hc = new HorarioConsultor(hid, pid);

        customizar?.Invoke(hc);

        return hc;
    }
}
