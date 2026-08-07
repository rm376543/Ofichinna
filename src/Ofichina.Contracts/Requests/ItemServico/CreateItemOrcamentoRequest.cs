using Ofichina.Contracts.Common;

namespace Ofichina.Contracts.Requests.ItensServico;

/// <summary>
/// Dados necessários para criação de um item de serviço no orçamento.
/// </summary>
public sealed class CreateItemOrcamentoRequest : CreateRequest
{
    /// <summary>
    /// Identificador do orçamento.
    /// </summary>
    public Guid OrcamentoId { get; init; }

    /// <summary>
    /// Serviço executado no orçamento.
    /// </summary>
    public Guid ServicoId { get; init; }

    /// <summary>
    /// Peça utilizada no serviço (opcional).
    /// </summary>
    public Guid? PecaId { get; init; }

    /// <summary>
    /// Quantidade de peças utilizadas.
    /// </summary>
    public int Quantidade { get; init; }
}
