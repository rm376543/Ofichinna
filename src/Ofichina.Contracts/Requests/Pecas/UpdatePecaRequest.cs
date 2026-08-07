using Ofichina.Contracts.Requests;

namespace Ofichina.Contracts.Requests.Pecas;

/// <summary>
/// Dados necessários para atualização de uma peça.
/// </summary>
public sealed class UpdatePecaRequest : UpdateRequest
{
    /// <summary>
    /// Identificador da peça.
    /// </summary>
    public Guid PecaId { get; init; }

    /// <summary>
    /// Nome da peça.
    /// </summary>
    public string Nome { get; init; } = string.Empty;

    /// <summary>
    /// Descrição da peça.
    /// </summary>
    public string? Descricao { get; init; }

    /// <summary>
    /// Código interno da peça.
    /// </summary>
    public string Codigo { get; init; } = string.Empty;

    /// <summary>
    /// Valor da peça.
    /// </summary>
    public decimal Valor { get; init; }

    /// <summary>
    /// Quantidade em estoque.
    /// </summary>
    public int QuantidadeEstoque { get; init; }
}