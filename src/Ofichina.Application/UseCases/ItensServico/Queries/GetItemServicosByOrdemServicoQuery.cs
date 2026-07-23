using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.ItensServico;

namespace Ofichina.Application.UseCases.ItensServico.Queries;

/// <summary>
/// Query para listar os itens de serviço de uma ordem de serviço.
/// </summary>
public sealed class GetItemServicosByOrdemServicoQuery : IQuery<Result<IReadOnlyCollection<ItemServicoResponse>>>
{
    /// <summary>
    /// Identificador da ordem de serviço.
    /// </summary>
    public Guid OrdemServicoId { get; init; }
}
