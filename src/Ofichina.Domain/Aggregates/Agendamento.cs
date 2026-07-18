using Ofichina.Domain.Entities;
using Ofichina.Domain.Exceptions;

namespace Ofichina.Domain.Aggregates;

/// <summary>
/// Representa um agendamento simples entre cliente, consultor e veículo.
/// </summary>
public class Agendamento : Entity
{
    /// <summary>
    /// Pessoa cliente vinculada ao agendamento.
    /// </summary>
    public Guid ClientePessoaId { get; private set; }

    /// <summary>
    /// Pessoa consultora vinculada ao agendamento.
    /// </summary>
    public Guid ConsultorPessoaId { get; private set; }

    /// <summary>
    /// Veículo informado para o agendamento.
    /// </summary>
    public Guid VeiculoId { get; private set; }

    /// <summary>
    /// Data do agendamento.
    /// </summary>
    public DateOnly DataAgendamento { get; private set; }

    /// <summary>
    /// Horário do agendamento.
    /// </summary>
    public TimeOnly HorarioAgendamento { get; private set; }

    /// <summary>
    /// Descrição opcional do agendamento.
    /// </summary>
    public string? Descricao { get; private set; }

    private Agendamento()
    {
    }

    public Agendamento(
        Guid clientePessoaId,
        Guid consultorPessoaId,
        Guid veiculoId,
        DateOnly dataAgendamento,
        TimeOnly horarioAgendamento,
        string? descricao = null)
    {
        if (clientePessoaId == Guid.Empty)
            throw new DomainException("A pessoa é obrigatória.");

        if (consultorPessoaId == Guid.Empty)
            throw new DomainException("O consultor é obrigatório.");

        if (veiculoId == Guid.Empty)
            throw new DomainException("O veículo é obrigatório.");

        if (dataAgendamento == default)
            throw new DomainException("A data do agendamento é obrigatória.");

        if (horarioAgendamento == default)
            throw new DomainException("O horário do agendamento é obrigatório.");

        ClientePessoaId = clientePessoaId;
        ConsultorPessoaId = consultorPessoaId;
        VeiculoId = veiculoId;
        DataAgendamento = dataAgendamento;
        HorarioAgendamento = horarioAgendamento;
        Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
    }
}