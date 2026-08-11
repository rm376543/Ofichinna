using Ofichina.Contracts.Common;

namespace Ofichina.Contracts.Requests.ItensServico;

/// <summary>
/// Dados necessários para atualização de um item de serviço somente-serviço no orçamento.
/// </summary>
public sealed class UpdateServicoOrcamentoRequest : UpdateRequest
{
    /// <summary>
    /// Identificador do item de serviço.
    /// </summary>
    public Guid ItemServicoId { get; init; } = Guid.Empty;

    /// <summary>
    /// Identificador do orçamento.
    /// </summary>
    public Guid OrcamentoId { get; init; } = Guid.Empty;

    /// <summary>
    /// Identificador do serviço executado.
    /// </summary>
    public Guid ServicoId { get; init; } = Guid.Empty;
}