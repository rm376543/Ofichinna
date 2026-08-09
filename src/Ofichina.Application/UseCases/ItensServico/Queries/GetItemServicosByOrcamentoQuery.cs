using Ofichina.Application.Abstractions;
using Ofichina.Contracts.Common;
using Ofichina.Contracts.Responses.Orcamento;

namespace Ofichina.Application.UseCases.ItensServico.Queries;

/// <summary>
/// Query para obter os itens de serviço de um orçamento.
/// </summary>
public sealed class GetItemServicosByOrcamentoQuery : IQuery<Result<IReadOnlyCollection<OrcamentoItemResponse>>>
{
    /// <summary>
    /// Identificador do orçamento.
    /// </summary>
    public Guid OrcamentoId { get; init; }
}
