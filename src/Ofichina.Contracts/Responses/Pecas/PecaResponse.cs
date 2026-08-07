using Ofichina.Contracts.Common;

namespace Ofichina.Contracts.Responses.Pecas;

/// <summary>
/// Resposta com os dados de uma peça.
/// </summary>
public sealed class PecaResponse : BaseEntity
{
    public Guid PecaId { get; set; }

    /// <summary>
    /// Nome da peça.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Descrição detalhada da peça.
    /// </summary>
    public string? Descricao { get; set; }

    /// <summary>
    /// Código interno da peça.
    /// </summary>
    public string Codigo { get; set; } = string.Empty;

    /// <summary>
    /// Valor unitário da peça.
    /// </summary>
    public decimal Valor { get; set; }

    /// <summary>
    /// Quantidade disponível em estoque.
    /// </summary>
    public int QuantidadeEstoque { get; set; }
}