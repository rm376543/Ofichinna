using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Permissoes;

namespace Ofichina.Application.UseCases.Permissoes.Queries;

public sealed class GetAllPermissoesPaginadasQuery : IQuery<Result<PagedResponse<PermissaoResponse>>>
{
    public Pagination Pagination { get; }

    public GetAllPermissoesPaginadasQuery(Pagination pagination)
    {
        Pagination = pagination;
    }
}
