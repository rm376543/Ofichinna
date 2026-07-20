namespace Ofichina.Contracts.Responses.Agendamento;

/// <summary>
/// Resposta com os dados de um dia de disponibilidade.
/// </summary>
public sealed class DiaDisponibilidadeResponse
{
    public Guid Id { get; set; }

    public DateOnly Data { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}