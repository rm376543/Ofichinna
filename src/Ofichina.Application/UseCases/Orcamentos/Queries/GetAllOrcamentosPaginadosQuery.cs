using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Orcamento;

namespace Ofichina.Application.UseCases.Orcamentos.Queries;

/// <summary>
/// Query para listar orçamentos de forma paginada.
/// </summary>
public sealed class GetAllOrcamentosPaginadosQuery : IQuery<Result<PagedResponse<OrcamentoDetalheResponse>>>
{
    public Pagination Pagination { get; }

    public GetAllOrcamentosPaginadosQuery(Pagination pagination)
    {
        Pagination = pagination;
    }
}
