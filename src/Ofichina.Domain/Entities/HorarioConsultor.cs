using Ofichina.Domain.Exceptions;

namespace Ofichina.Domain.Entities;

/// <summary>
/// Representa o vínculo entre um consultor e um horário de disponibilidade.
/// </summary>
public sealed class HorarioConsultor : Entity
{
    /// <summary>
    /// Identificador do horário disponível vinculado.
    /// </summary>
    public Guid HorarioDisponibilidadeId { get; private set; }

    /// <summary>
    /// Horário disponível vinculado.
    /// </summary>
    public HorarioDisponibilidade HorarioDisponibilidade { get; private set; } = null!;

    /// <summary>
    /// Identificador da pessoa consultora.
    /// </summary>
    public Guid PessoaId { get; private set; }

    /// <summary>
    /// Pessoa consultora vinculada.
    /// </summary>
    public Pessoa Pessoa { get; private set; } = null!;

    private HorarioConsultor()
    {
    }

    public HorarioConsultor(Guid horarioDisponibilidadeId, Guid pessoaId)
    {
        if (horarioDisponibilidadeId == Guid.Empty)
            throw new DomainException("O horário disponível deve ser informado.");

        if (pessoaId == Guid.Empty)
            throw new DomainException("A pessoa consultora deve ser informada.");

        HorarioDisponibilidadeId = horarioDisponibilidadeId;
        PessoaId = pessoaId;
    }
}