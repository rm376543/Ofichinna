using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Agendamento;

namespace Ofichina.Application.UseCases.Agendamentos.Queries;

/// <summary>
/// Consulta para obter um agendamento por Id.
/// </summary>
public sealed class GetAgendamentoByIdQuery : IQuery<Result<AgendamentoUsuarioDetalheResponse>>
{
    public Guid PessoaId { get; init; }

    public Guid Id { get; init; }

    public GetAgendamentoByIdQuery(Guid pessoaId, Guid id)
    {
        PessoaId = pessoaId;
        Id = id;
    }
}


