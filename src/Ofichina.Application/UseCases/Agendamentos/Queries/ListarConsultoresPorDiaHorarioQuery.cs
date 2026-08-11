using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Agendamento.Consultor;

namespace Ofichina.Application.UseCases.Agendamentos.Queries;

/// <summary>
/// Query para listar consultores disponíveis para dia + horário.
/// </summary>
public sealed class ListarConsultoresPorDiaHorarioQuery : IQuery<Result<IEnumerable<ConsultorDisponibilidadeResponse>>>
{
    public Guid DiaDisponibilidadeId { get; init; }
    public Guid HorarioDisponibilidadeId { get; init; }
}

