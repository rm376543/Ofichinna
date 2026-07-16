namespace Ofichina.Contracts.Responses.Servicos;

/// <summary>
/// Resposta com os dados completos de um serviço.
/// </summary>
public sealed class ServicoResponse
{
    /// <summary>
    /// Identificador do serviço.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nome do serviço.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Descrição do serviço.
    /// </summary>
    public string? Descricao { get; set; }

    /// <summary>
    /// Valor cobrado pelo serviço.
    /// </summary>
    public decimal Valor { get; set; }

    /// <summary>
    /// Indica se o serviço está ativo.
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