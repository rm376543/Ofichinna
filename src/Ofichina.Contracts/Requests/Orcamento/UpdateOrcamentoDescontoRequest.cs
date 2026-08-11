namespace Ofichina.Contracts.Requests.Orcamento;

/// <summary>
/// Requisição para atualização do desconto de um orçamento.
/// </summary>
public sealed class UpdateOrcamentoDescontoRequest
{
    public Guid OrcamentoId { get; set; }
    /// <summary>
    /// Valor do desconto.
    /// </summary>
    public decimal Desconto { get; init; }

    /// <summary>
    /// Indica se o desconto é percentual.
    /// </summary>
    public bool DescontoEmDinheiro { get; init; }

}
