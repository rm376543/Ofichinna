using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.OrdemServico;
using Ofichina.Domain.Common;

namespace Ofichina.Application.UseCases.OrdensServico.Queries;

/// <summary>
/// Consulta para listar ordens de serviço.
/// </summary>
public sealed class GetOrdensServicoQuery : IQuery<Result<IReadOnlyCollection<OrdemServicoResponse>>>
{
    public Pagination Pagination { get; init; } = new();
}
