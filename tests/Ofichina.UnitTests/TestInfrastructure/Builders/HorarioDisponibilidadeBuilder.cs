using Ofichina.Domain.Entities;

namespace Ofichina.UnitTests.TestInfrastructure.Builders;

public class HorarioDisponibilidadeBuilder
{
    private HorarioDisponibilidade _horario;

    public HorarioDisponibilidadeBuilder()
    {
        _horario = TestDataFactory.HorariosDisponibilidade.Criar();
    }

    public HorarioDisponibilidadeBuilder ComId(Guid id)
    {
        ReflectionHelpers.DefinirId(_horario, id);
        return this;
    }

    public HorarioDisponibilidadeBuilder ComHora(TimeOnly hora)
    {
        _horario.AlterarHora(hora);
        return this;
    }

    public HorarioDisponibilidadeBuilder VincularConsultor(HorarioConsultor consultor)
    {
        _horario.VincularConsultor(consultor);
        return this;
    }

    public HorarioDisponibilidadeBuilder VincularDia(DiaHorarioDisponibilidade diaHorario)
    {
        _horario.VincularDia(diaHorario);
        return this;
    }

    public HorarioDisponibilidade Build() => _horario;
}
