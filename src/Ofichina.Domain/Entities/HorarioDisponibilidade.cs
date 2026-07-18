using Ofichina.Domain.Exceptions;

namespace Ofichina.Domain.Entities;

/// <summary>
/// Representa um horário disponível em um dia específico.
/// </summary>
public sealed class HorarioDisponibilidade : Entity
{
    private readonly List<HorarioConsultor> _consultores = [];

    private readonly List<DiaHorarioDisponibilidade> _dias = [];

    /// <summary>
    /// Hora disponível.
    /// </summary>
    public TimeOnly Hora { get; private set; }

    /// <summary>
    /// Consultores vinculados a este horário.
    /// </summary>
    public IReadOnlyCollection<HorarioConsultor> Consultores => _consultores.AsReadOnly();

    /// <summary>
    /// Vínculos entre este horário e os dias disponíveis.
    /// </summary>
    public IReadOnlyCollection<DiaHorarioDisponibilidade> Dias => _dias.AsReadOnly();

    private HorarioDisponibilidade()
    {
    }

    public HorarioDisponibilidade(TimeOnly hora)
    {
        Hora = hora;
    }

    public void AlterarHora(TimeOnly hora)
    {
        Hora = hora;
        AtualizarDataModificacao();
    }

    public void VincularConsultor(HorarioConsultor consultor)
    {
        ArgumentNullException.ThrowIfNull(consultor);

        _consultores.Add(consultor);
        AtualizarDataModificacao();
    }

    public void VincularDia(DiaHorarioDisponibilidade diaHorario)
    {
        ArgumentNullException.ThrowIfNull(diaHorario);

        _dias.Add(diaHorario);
        AtualizarDataModificacao();
    }
}