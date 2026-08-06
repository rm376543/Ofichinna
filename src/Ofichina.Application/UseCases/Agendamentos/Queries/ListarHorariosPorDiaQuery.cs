using Ofichina.Application.Abstractions;

namespace Ofichina.Application.UseCases.Agendamentos.Queries;

/// <summary>
/// Query para listar horários disponíveis de um dia.
/// </summary>
public sealed class ListarHorariosPorDiaQuery : IQuery<IEnumerable<HorarioListaDto>>
{
    public Guid DiaDisponibilidadeId { get; init; }
}

/// <summary>
/// DTO com informações do horário.
/// </summary>
public sealed class HorarioListaDto
{
    public Guid Id { get; set; }
    public string Hora { get; set; } = string.Empty; // HH:mm
    public bool Disponivel { get; set; }
}
