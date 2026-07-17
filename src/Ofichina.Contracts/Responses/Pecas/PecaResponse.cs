namespace Ofichina.Contracts.Responses.Pecas;

/// <summary>
/// Resposta com os dados de uma peça.
/// </summary>
public sealed class PecaResponse
{
    /// <summary>
    /// Identificador da peça.
    /// </summary>
    public Guid Id { get; set; }

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

    /// <summary>
    /// Indica se a peça está ativa.
    /// </summary>
    public bool Ativo { get; set; }

    /// <summary>
    /// Data de criação do registro.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Data da última atualização.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Data da exclusão lógica.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}