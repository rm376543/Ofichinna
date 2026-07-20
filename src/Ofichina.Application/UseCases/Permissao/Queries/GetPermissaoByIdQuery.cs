using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Permissao;

namespace Ofichina.Application.UseCases.Permissoes.Queries;

public sealed class GetPermissaoByIdQuery : IQuery<Result<PermissaoResponse>>
{
    public Guid Id { get; }

    public GetPermissaoByIdQuery(Guid id)
    {
        Id = id;
    }
}
