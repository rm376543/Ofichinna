using Ofichina.Contracts.Requests;

namespace Ofichina.Contracts.Requests.Agendamento;

/// <summary>
/// Requisição para criação de agendamento pelo aplicativo.
/// </summary>
public sealed class CreateAgendamentoRequest : CreateRequest
{
    /// <summary>
    /// Identificador da pessoa consultora.
    /// </summary>
    public Guid ConsultorPessoaId { get; init; }

    /// <summary>
    /// Identificador do veículo a ser atendido.
    /// </summary>
    public Guid VeiculoId { get; init; }

    /// <summary>
    /// Data do agendamento.
    /// </summary>
    public DateOnly DataAgendamento { get; init; }

    /// <summary>
    /// Horário do agendamento.
    /// </summary>
    public TimeOnly HorarioAgendamento { get; init; }

    /// <summary>
    /// Descrição opcional do agendamento.
    /// </summary>
    public string? Descricao { get; init; }
}