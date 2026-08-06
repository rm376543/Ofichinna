namespace Ofichina.Contracts.Requests.Agendamento;

/// <summary>
/// Requisição para criação de agendamento usando o novo modelo com HorarioConsultorDisponibilidade.
/// </summary>
public sealed class CreateAgendamentoRequest : CreateRequest
{
    /// <summary>
    /// Identificador único do agendamento.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Identificador único da pessoa que está solicitando o agendamento.
    /// </summary>
    public Guid PessoaId { get; set; }
    /// <summary>
    /// Identificador do slot de disponibilidade (HorarioConsultorDisponibilidade) que consolida dia + horário + consultor.
    /// </summary>
    public Guid HorarioConsultorDisponibilidadeId { get; init; }

    /// <summary>
    /// Identificador do veículo a ser atendido.
    /// </summary>
    public Guid VeiculoId { get; init; }

    /// <summary>
    /// Descrição opcional do agendamento.
    /// </summary>
    public string? Descricao { get; init; }
}