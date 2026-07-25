using Ofichina.Application.Abstractions;
using Ofichina.Contracts;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.PerfilPermissoes;

namespace Ofichina.Application.UseCases.PerfilPermissoes.Queries;

public sealed class GetAllPerfisPermissoesPaginadasQuery : IQuery<Result<PagedResponse<PerfilPermissaoResponse>>>
{
    public Guid PerfilId { get; }
    public Pagination Pagination { get; set; }

    public GetAllPerfisPermissoesPaginadasQuery(Guid perfilId, Pagination pagination)
    {
        PerfilId = perfilId;
        Pagination = pagination;
    }
}
