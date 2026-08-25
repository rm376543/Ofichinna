using Ofichina.Domain.Entities;

namespace Ofichina.UnitTests.TestInfrastructure.Builders;

public class DiaDisponibilidadeBuilder
{
    private DiaDisponibilidade _dia;

    public DiaDisponibilidadeBuilder()
    {
        _dia = TestDataFactory.DiasDisponibilidade.Criar();
    }

    public DiaDisponibilidadeBuilder ComId(Guid id)
    {
        ReflectionHelpers.DefinirId(_dia, id);
        return this;
    }

    public DiaDisponibilidadeBuilder ComData(DateOnly data)
    {
        _dia.AlterarData(data);
        return this;
    }

    public DiaDisponibilidadeBuilder AdicionarHorario(DiaHorarioDisponibilidade horario)
    {
        _dia.AdicionarHorario(horario);
        return this;
    }

    public DiaDisponibilidade Build() => _dia;
}
