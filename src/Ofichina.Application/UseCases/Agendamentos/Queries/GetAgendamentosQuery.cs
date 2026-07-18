using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Agendamento;

namespace Ofichina.Application.UseCases.Agendamentos.Queries;

/// <summary>
/// Consulta para listar agendamentos de uma pessoa específica.
/// </summary>
public sealed class GetAgendamentosQuery : IQuery<Result<IReadOnlyCollection<AgendamentoResponse>>>
{
    public Guid PessoaId { get; init; }

    public Pagination Pagination { get; init; } = new();

    public GetAgendamentosQuery(Guid pessoaId)
    {
        PessoaId = pessoaId;
    }
}