using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Pecas;

namespace Ofichina.Application.UseCases.Pecas.Queries;

/// <summary>
/// Consulta para obter uma peça por Id.
/// </summary>
public sealed class GetPecaByIdQuery : IQuery<Result<PecaResponse>>
{
    /// <summary>
    /// Identificador da peça.
    /// </summary>
    public Guid Id { get; init; }
}