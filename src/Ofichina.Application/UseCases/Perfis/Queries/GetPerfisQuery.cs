using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Perfis;
using Ofichina.Domain.Common;

namespace Ofichina.Application.UseCases.Perfis.Queries;

/// <summary>
/// Query para listar perfis.
/// </summary>
public class GetPerfisQuery : IQuery<Result<IReadOnlyCollection<PerfilResponse>>>
{
    public Pagination Pagination { get; init; } = new();
}
