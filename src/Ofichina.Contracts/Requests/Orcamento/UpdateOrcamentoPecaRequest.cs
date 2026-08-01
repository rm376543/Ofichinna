namespace Ofichina.Contracts.Requests.Orcamentos;

/// <summary>
/// Peça atualizada no orçamento.
/// </summary>
public sealed class UpdateOrcamentoPecaRequest : UpdateRequest
{
    /// <summary>
    /// Peça cadastrada.
    /// </summary>
    public Guid PecaId { get; init; }

    /// <summary>
    /// Quantidade.
    /// </summary>
    public decimal Quantidade { get; init; }
}