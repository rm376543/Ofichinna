namespace Ofichina.Contracts.Requests.Orcamentos;

/// <summary>
/// Peça atualizada no serviço do orçamento.
/// </summary>
public sealed class UpdateOrcamentoServicoPecaRequest : UpdateRequest
{
    /// <summary>
    /// Peça cadastrada.
    /// </summary>
    public Guid PecaId { get; init; }

    /// <summary>
    /// Quantidade.
    /// </summary>
    public int Quantidade { get; init; }
}
