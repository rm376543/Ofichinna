namespace Ofichina.Contracts.Requests.Agendamento;

/// <summary>
/// Requisição para criação de agendamento usando o novo modelo com AgendaConsultor.
/// </summary>
public sealed class CreateAgendamentoRequest : CreateRequest
{
    /// <summary>
    /// Identificador único da pessoa que está solicitando o agendamento.
    /// </summary>
    public Guid PessoaId { get; set; }
    /// <summary>
    /// Identificador do slot de disponibilidade (AgendaConsultor) que consolida dia + horário + consultor.
    /// </summary>
    public Guid AgendaConsultorId { get; init; }

    /// <summary>
    /// Identificador do veículo a ser atendido.
    /// </summary>
    public Guid VeiculoId { get; init; }

    /// <summary>
    /// Descrição opcional do agendamento.
    /// </summary>
    public string? Descricao { get; init; }
}