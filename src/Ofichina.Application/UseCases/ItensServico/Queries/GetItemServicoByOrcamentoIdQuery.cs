using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Orcamento;

namespace Ofichina.Application.UseCases.ItensServico.Queries;

/// <summary>
/// Query para obter um item de serviço específico de um orçamento.
/// </summary>
public sealed class GetItemServicoByOrcamentoIdQuery : IQuery<Result<OrcamentoItemResponse>>
{
    /// <summary>
    /// Identificador do orçamento.
    /// </summary>
    public Guid OrcamentoId { get; init; }

    /// <summary>
    /// Identificador do item de serviço.
    /// </summary>
    public Guid ItemServicoId { get; init; }

    public GetItemServicoByOrcamentoIdQuery(Guid orcamentoId, Guid itemServicoId)
    {
        OrcamentoId = orcamentoId;
        ItemServicoId = itemServicoId;
    }
}
