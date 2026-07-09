namespace Ofichina.Contracts.Requests.Orcamentos;

/// <summary>
/// Peça do orçamento.
/// </summary>
public sealed class CreateOrcamentoPecaRequest
{
    /// <summary>
    /// Peça cadastrada.
    /// </summary>
    public Guid PecaId { get; init; }

    /// <summary>
    /// Quantidade.
    /// </summary>
    public decimal Quantidade { get; init; }

    /// <summary>
    /// Valor unitário.
    /// </summary>
    public decimal ValorUnitario { get; init; }

    /// <summary>
    /// Desconto do item.
    /// </summary>
    public decimal Desconto { get; init; }
}