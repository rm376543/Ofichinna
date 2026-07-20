namespace Ofichina.Contracts.Responses.Agendamento;

/// <summary>
/// Resposta resumida com os dados de um consultor.
/// </summary>
public sealed class ConsultorResponse
{
    public Guid Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}