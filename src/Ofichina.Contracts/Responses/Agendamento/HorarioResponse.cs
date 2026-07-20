namespace Ofichina.Contracts.Responses.Agendamento;

/// <summary>
/// Resposta com os dados de um horário disponível.
/// </summary>
public sealed class HorarioResponse
{
    public Guid Id { get; set; }

    public TimeOnly Hora { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}