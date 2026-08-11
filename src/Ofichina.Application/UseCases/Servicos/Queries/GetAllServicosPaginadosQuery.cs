using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Servicos;

namespace Ofichina.Application.UseCases.Servicos.Queries;

/// <summary>
/// Consulta para listar serviços.
/// </summary>
public sealed class GetAllServicosPaginadosQuery : IQuery<Result<PagedResponse<ServicoResponse>>>
{
    public Pagination Pagination { get; }

    public GetAllServicosPaginadosQuery(Pagination pagination)
    {
        Pagination = pagination;
    }
}