namespace Ofichina.Contracts.Responses.Agendamento;

/// <summary>
/// Response com slot de agenda do consultor.
/// </summary>
public sealed class AgendaConsultorResponse
{
    public Guid AgendaConsultorId { get; set; }
    public string Hora { get; set; } = string.Empty;
    public string Status { get; set; } = "VAGO";
    public string? ClienteNome { get; set; }
    public string? Veiculo { get; set; }
}
