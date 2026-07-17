namespace Ofichina.Contracts.Responses.Agendamento;

/// <summary>
/// Resposta com os dados de um agendamento.
/// </summary>
public sealed class AgendamentoResponse
{
    public Guid Id { get; set; }

    public Guid PessoaId { get; set; }

    public Guid VeiculoId { get; set; }

    public DateTime DataHoraAgendada { get; set; }

    public string Motivo { get; set; } = string.Empty;

    public string? Observacoes { get; set; }

    public string Status { get; set; } = string.Empty;

    public string CanalAtendimento { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }
}