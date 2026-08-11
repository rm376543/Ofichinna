using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Agendamento;

namespace Ofichina.Application.UseCases.Agendamentos.Queries;

/// <summary>
/// Consulta para listar agendamentos de uma pessoa específica.
/// </summary>
public sealed class GetAgendamentosQuery : IQuery<Result<IReadOnlyCollection<AgendamentoUsuarioResponse>>>
{
    public Guid PessoaId { get; init; }

    public GetAgendamentosQuery(Guid pessoaId)
    {
        PessoaId = pessoaId;
    }
}