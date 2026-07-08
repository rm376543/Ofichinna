namespace Ofichina.Contracts.Requests.Orcamentos;

/// <summary>
/// Peça atualizada no orçamento.
/// </summary>
public sealed class UpdateOrcamentoPecaRequest : UpdateRequest
{
    /// <summary>
    /// Quantidade.
    /// </summary>
    public decimal Quantidade { get; init; }
}