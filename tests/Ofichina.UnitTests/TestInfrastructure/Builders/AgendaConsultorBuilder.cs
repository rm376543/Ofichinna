using Ofichina.Domain.Entities;

namespace Ofichina.UnitTests.TestInfrastructure.Builders;

public class AgendaConsultorBuilder
{
    private AgendaConsultor _agenda;

    public AgendaConsultorBuilder()
    {
        _agenda = TestDataFactory.AgendaConsultores.Criar();
    }

    public AgendaConsultorBuilder ComId(Guid id)
    {
        ReflectionHelpers.DefinirId(_agenda, id);
        return this;
    }

    public AgendaConsultorBuilder ComDiaDisponibilidadeId(Guid diaId)
    {
        ReflectionHelpers.DefinirPropriedade(_agenda, "DiaDisponibilidadeId", diaId);
        return this;
    }

    public AgendaConsultorBuilder ComHorarioDisponibilidadeId(Guid horarioId)
    {
        ReflectionHelpers.DefinirPropriedade(_agenda, "HorarioDisponibilidadeId", horarioId);
        return this;
    }

    public AgendaConsultorBuilder ComConsultorPessoaId(Guid consultorId)
    {
        ReflectionHelpers.DefinirPropriedade(_agenda, "ConsultorPessoaId", consultorId);
        return this;
    }

    public AgendaConsultor Build() => _agenda;
}
