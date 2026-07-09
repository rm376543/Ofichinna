namespace Ofichina.Contracts.Requests.Orcamentos;

/// <summary>
/// Serviço do orçamento.
/// </summary>
public sealed class CreateOrcamentoServicoRequest : CreateRequest
{
    /// <summary>
    /// Serviço cadastrado.
    /// </summary>
    public Guid ServicoId { get; init; }

    /// <summary>
    /// Quantidade.
    /// </summary>
    public decimal Quantidade { get; init; }

    /// <summary>
    /// Valor unitário.
    /// </summary>
    public decimal ValorUnitario { get; init; }

    /// <summary>
    /// Observações.
    /// </summary>
    public string? Observacoes { get; init; }
}