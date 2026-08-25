using Bogus;
using Ofichina.Domain.Entities;

namespace Ofichina.UnitTests.TestInfrastructure.Fakers;

public class AgendaConsultorFaker
{
    private readonly Faker _faker = new();

    public AgendaConsultor Criar(Guid? diaDisponibilidadeId = null, Guid? horarioDisponibilidadeId = null, Guid? consultorPessoaId = null, Action<AgendaConsultor>? customizar = null)
    {
        var diaId = diaDisponibilidadeId ?? _faker.Random.Guid();
        var horarioId = horarioDisponibilidadeId ?? _faker.Random.Guid();
        var consultorId = consultorPessoaId ?? _faker.Random.Guid();

        var agenda = new AgendaConsultor(diaId, horarioId, consultorId);

        customizar?.Invoke(agenda);

        return agenda;
    }
}
