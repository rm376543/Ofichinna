using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Agendamento;

namespace Ofichina.Application.UseCases.Agendamentos.Queries;

/// <summary>
/// Query para listar horários disponíveis de um dia.
/// </summary>
public sealed class ListarHorariosPorDiaQuery : IQuery<Result<IEnumerable<HorarioDisponivelResponse>>>
{
    public Guid DiaDisponibilidadeId { get; init; }
}
