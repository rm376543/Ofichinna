using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Permissao;

namespace Ofichina.Application.UseCases.Permissoes.Queries;

public sealed class GetPermissoesQuery : IQuery<Result<IReadOnlyCollection<PermissaoResponse>>>
{
}
