using Ofichina.Contracts.Requests;

namespace Ofichina.Contracts.Requests.Agendamento;

/// <summary>
/// Requisição para criação de agendamento pelo aplicativo.
/// </summary>
public sealed class CreateAgendamentoRequest : CreateRequest
{
    /// <summary>
    /// Identificador do veículo a ser atendido.
    /// </summary>
    public Guid VeiculoId { get; init; }

    /// <summary>
    /// Data e hora preferenciais para o atendimento.
    /// </summary>
    public DateTime DataHoraPreferida { get; init; }

    /// <summary>
    /// Motivo principal do agendamento.
    /// </summary>
    public string Motivo { get; init; } = string.Empty;

    /// <summary>
    /// Observações adicionais informadas pelo cliente.
    /// </summary>
    public string? Observacoes { get; init; }
}