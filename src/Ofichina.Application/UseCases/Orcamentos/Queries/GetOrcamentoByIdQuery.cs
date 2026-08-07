using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Orcamento;

namespace Ofichina.Application.UseCases.Orcamentos.Queries;

/// <summary>
/// Query para obter um orçamento por identificador.
/// </summary>
public sealed class GetOrcamentoByIdQuery : IQuery<Result<OrcamentoResponse>>
{
    public Guid Id { get; init; }

    public GetOrcamentoByIdQuery(Guid id)
    {
        Id = id;
    }
}
