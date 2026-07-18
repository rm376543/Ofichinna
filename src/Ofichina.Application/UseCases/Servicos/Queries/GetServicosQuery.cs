using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Servicos;

namespace Ofichina.Application.UseCases.Servicos.Queries;

/// <summary>
/// Consulta para listar serviços.
/// </summary>
public sealed class GetServicosQuery : IQuery<Result<IReadOnlyCollection<ServicoResponse>>>
{
    public Pagination Pagination { get; init; } = new();
}