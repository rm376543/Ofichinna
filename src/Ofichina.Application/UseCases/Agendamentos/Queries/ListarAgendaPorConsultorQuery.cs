using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Agendamento;

namespace Ofichina.Application.UseCases.Agendamentos.Queries;

/// <summary>
/// Query para listar agenda de um consultor em uma data.
/// </summary>
public sealed class ListarAgendaPorConsultorQuery : IQuery<Result<IEnumerable<AgendaConsultorResponse>>>
{
    public Guid ConsultorPessoaId { get; init; }
    public DateOnly Data { get; init; }
}

