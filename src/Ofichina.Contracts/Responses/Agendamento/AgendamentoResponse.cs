namespace Ofichina.Contracts.Responses.Agendamento;

/// <summary>
/// Resposta com os dados de um agendamento.
/// </summary>
public sealed class AgendamentoResponse
{
    public Guid Id { get; set; }

    public Guid ClientePessoaId { get; set; }

    public string ClienteNome { get; set; } = string.Empty;

    public Guid DiaDisponibilidadeId { get; set; }

    public Guid HorarioConsultorId { get; set; }

    public Guid ConsultorPessoaId { get; set; }

    public string ConsultorNome { get; set; } = string.Empty;

    public Guid VeiculoId { get; set; }

    public string VeiculoPlaca { get; set; } = string.Empty;

    public string VeiculoDescricao { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string? Descricao { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }
}