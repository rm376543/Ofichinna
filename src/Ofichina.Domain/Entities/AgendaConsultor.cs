using Ofichina.Domain.Exceptions;

namespace Ofichina.Domain.Entities;

/// <summary>
/// Representa o vínculo entre dia disponível, horário disponível e consultor.
/// Consolida a disponibilidade em um único slot.
/// </summary>
public sealed class AgendaConsultor : Entity
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

    /// <summary>
    /// Identificador da pessoa consultora.
    /// </summary>
    public Guid ConsultorPessoaId { get; private set; }

    /// <summary>
    /// Pessoa consultora vinculada.
    /// </summary>
    public Pessoa Consultor { get; private set; } = null!;

    /// <summary>
    /// Construtor privado para uso interno da entidade.
    /// </summary>
    private AgendaConsultor()
    {
    }

    /// <summary>
    /// Cria uma nova instância de AgendaConsultor.
    /// </summary>
    /// <param name="diaDisponibilidadeId">ID do dia disponível.</param>
    /// <param name="horarioDisponibilidadeId">ID do horário disponível.</param>
    /// <param name="consultorPessoaId">ID da pessoa consultora.</param>
    /// <exception cref="DomainException"></exception>
    public AgendaConsultor(
        Guid diaDisponibilidadeId,
        Guid horarioDisponibilidadeId,
        Guid consultorPessoaId)
    {
        if (diaDisponibilidadeId == Guid.Empty)
            throw new DomainException("O dia disponível deve ser informado.");

        if (horarioDisponibilidadeId == Guid.Empty)
            throw new DomainException("O horário disponível deve ser informado.");

        if (consultorPessoaId == Guid.Empty)
            throw new DomainException("A pessoa consultora deve ser informada.");

        DiaDisponibilidadeId = diaDisponibilidadeId;
        HorarioDisponibilidadeId = horarioDisponibilidadeId;
        ConsultorPessoaId = consultorPessoaId;
    }
}
