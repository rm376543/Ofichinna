using Ofichina.Domain.Entities;

namespace Ofichina.UnitTests.TestInfrastructure.Builders;

public class ChecklistBuilder
{
    private Checklist _checklist;

    public ChecklistBuilder()
    {
        _checklist = TestDataFactory.Checklists.Criar();
    }

    public ChecklistBuilder ComId(Guid id)
    {
        ReflectionHelpers.DefinirId(_checklist, id);
        return this;
    }

    public ChecklistBuilder ComAgendamentoId(Guid agendamentoId)
    {
        _checklist.VincularAgendamento(agendamentoId);
        return this;
    }

    public ChecklistBuilder ComItensVerificados(string itens)
    {
        ReflectionHelpers.DefinirPropriedade(_checklist, "ItensVerificados", itens);
        return this;
    }

    public ChecklistBuilder Finalizado()
    {
        if (!_checklist.EstaFinalizado())
            _checklist.Finalizar();
        return this;
    }

    public ChecklistBuilder Reabrir()
    {
        _checklist.Reabrir();
        return this;
    }

    public Checklist Build() => _checklist;
}
