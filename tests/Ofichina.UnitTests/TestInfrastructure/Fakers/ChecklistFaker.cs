using Bogus;
using Ofichina.Domain.Entities;

namespace Ofichina.UnitTests.TestInfrastructure.Fakers;

public class ChecklistFaker
{
    private readonly Faker _faker = new();

    public Checklist Criar(Guid? agendamentoId = null, Action<Checklist>? customizar = null)
    {
        var agId = agendamentoId ?? Guid.NewGuid();
        var itens = _faker.Lorem.Sentence();
        var obs = _faker.Lorem.Sentence();

        var checklist = new Checklist(agId, itens, obs);

        customizar?.Invoke(checklist);

        return checklist;
    }
}
