using Ofichina.Domain.Exceptions;

namespace Ofichina.Domain.Entities;

/// <summary>
/// Representa o vínculo entre um dia disponível e um horário disponível.
/// </summary>
public sealed class DiaHorarioDisponibilidade : Entity
{
    /// <summary>
    /// Identificador do dia disponível.
    /// </summary>
    public Guid DiaDisponibilidadeId { get; private set; }

    /// <summary>
    /// Dia disponível vinculado.
    /// </summary>
    public DiaDisponibilidade DiaDisponibilidade { get; private set; } = null!;

    /// <summary>
    /// Identificador do horário disponível.
    /// </summary>
    public Guid HorarioDisponibilidadeId { get; private set; }

    /// <summary>
    /// Horário disponível vinculado.
    /// </summary>
    public HorarioDisponibilidade HorarioDisponibilidade { get; private set; } = null!;

    private DiaHorarioDisponibilidade()
    {
    }

    public DiaHorarioDisponibilidade(Guid diaDisponibilidadeId, Guid horarioDisponibilidadeId)
    {
        if (diaDisponibilidadeId == Guid.Empty)
            throw new DomainException("O dia disponível deve ser informado.");

        if (horarioDisponibilidadeId == Guid.Empty)
            throw new DomainException("O horário disponível deve ser informado.");

        DiaDisponibilidadeId = diaDisponibilidadeId;
        HorarioDisponibilidadeId = horarioDisponibilidadeId;
    }
}