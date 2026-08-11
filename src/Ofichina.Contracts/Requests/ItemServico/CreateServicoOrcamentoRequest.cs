using Ofichina.Contracts.Common;

namespace Ofichina.Contracts.Requests.ItensServico;

/// <summary>
/// Dados necessários para criação de um item de serviço somente-serviço no orçamento.
/// </summary>
public sealed class CreateServicoOrcamentoRequest : CreateRequest
{
    /// <summary>
    /// Identificador do orçamento.
    /// </summary>
    public Guid OrcamentoId { get; init; }

    /// <summary>
    /// Identificador do serviço executado.
    /// </summary>
    public Guid ServicoId { get; init; }
}