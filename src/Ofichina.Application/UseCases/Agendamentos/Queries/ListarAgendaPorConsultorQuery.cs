using Ofichina.Application.Abstractions;

namespace Ofichina.Application.UseCases.Agendamentos.Queries;

/// <summary>
/// Query para listar agenda de um consultor em uma data.
/// </summary>
public sealed class ListarAgendaPorConsultorQuery : IQuery<IEnumerable<AgendaSlotDto>>
{
    public Guid ConsultorPessoaId { get; init; }
    public DateOnly Data { get; init; }
}

/// <summary>
/// DTO com slot de agenda do consultor.
/// </summary>
public sealed class AgendaSlotDto
{
    public Guid SlotId { get; set; }
    public string Hora { get; set; } = string.Empty; // HH:mm
    public string Status { get; set; } = "VAGO"; // VAGO, AGENDADO, INICIADO, FINALIZADO, CANCELADO
    public string? ClienteNome { get; set; }
    public string? Veiculo { get; set; }
}
