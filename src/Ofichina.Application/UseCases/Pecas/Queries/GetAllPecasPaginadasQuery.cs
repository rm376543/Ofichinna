using Ofichina.Application.Abstractions;
using Ofichina.Contracts;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Pecas;
using Ofichina.Domain.Common;

namespace Ofichina.Application.UseCases.Pecas.Queries;

/// <summary>
/// Consulta para listar peças.
/// </summary>
public sealed class GetAllPecasPaginadasQuery : IQuery<Result<PagedResponse<PecaResponse>>>
{
    public Pagination Pagination { get; }

    public GetAllPecasPaginadasQuery(Pagination pagination)
    {
        Pagination = pagination;
    }
}