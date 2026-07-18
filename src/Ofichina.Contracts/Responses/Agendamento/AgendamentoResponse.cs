namespace Ofichina.Contracts.Responses.Agendamento;

/// <summary>
/// Resposta com os dados de um agendamento.
/// </summary>
public sealed class AgendamentoResponse
{
    public Guid Id { get; set; }

    public Guid ClientePessoaId { get; set; }

    public Guid ConsultorPessoaId { get; set; }

    public Guid VeiculoId { get; set; }

    public DateOnly DataAgendamento { get; set; }

    public TimeOnly HorarioAgendamento { get; set; }

    public string? Descricao { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }
}