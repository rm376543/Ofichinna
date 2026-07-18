using Ofichina.Domain.Exceptions;

namespace Ofichina.Domain.Entities;

/// <summary>
/// Representa um dia disponível para atendimento.
/// </summary>
public sealed class DiaDisponibilidade : Entity
{
    private readonly List<DiaHorarioDisponibilidade> _horarios = [];

    /// <summary>
    /// Data do dia disponível.
    /// </summary>
    public DateOnly Data { get; private set; }

    /// <summary>
    /// Horários vinculados a este dia.
    /// </summary>
    public IReadOnlyCollection<DiaHorarioDisponibilidade> Horarios => _horarios.AsReadOnly();

    private DiaDisponibilidade()
    {
    }

    public DiaDisponibilidade(DateOnly data)
    {
        if (data == default)
            throw new DomainException("A data do dia disponível deve ser informada.");

        Data = data;
    }

    public void AlterarData(DateOnly data)
    {
        if (data == default)
            throw new DomainException("A data do dia disponível deve ser informada.");

        Data = data;
        AtualizarDataModificacao();
    }

    public void AdicionarHorario(DiaHorarioDisponibilidade horario)
    {
        ArgumentNullException.ThrowIfNull(horario);

        _horarios.Add(horario);
        AtualizarDataModificacao();
    }
}