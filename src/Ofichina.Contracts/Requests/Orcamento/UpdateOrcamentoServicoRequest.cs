namespace Ofichina.Contracts.Requests.Orcamentos;

/// <summary>
/// Serviço atualizado no orçamento.
/// </summary>
public sealed class UpdateOrcamentoServicoRequest : UpdateRequest
{
    /// <summary>
    /// Serviço.
    /// </summary>
    public Guid ServicoId { get; init; }

    /// <summary>
    /// Quantidade.
    /// </summary>
    public decimal Quantidade { get; init; }
}