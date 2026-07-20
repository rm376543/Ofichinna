using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Pecas;
using Ofichina.Domain.Common;

namespace Ofichina.Application.UseCases.Pecas.Queries;

/// <summary>
/// Consulta para listar peças.
/// </summary>
public sealed class GetPecasQuery : IQuery<Result<IReadOnlyCollection<PecaResponse>>>
{
    public Pagination Pagination { get; init; } = new();
}