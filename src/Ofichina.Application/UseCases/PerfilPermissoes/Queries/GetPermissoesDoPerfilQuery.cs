using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.PerfilPermissoes;

namespace Ofichina.Application.UseCases.PerfilPermissoes.Queries;

public sealed class GetPermissoesDoPerfilQuery : IQuery<Result<IReadOnlyCollection<PerfilPermissaoResponse>>>
{
    public Guid PerfilId { get; }

    public GetPermissoesDoPerfilQuery(Guid perfilId)
    {
        PerfilId = perfilId;
    }
}
