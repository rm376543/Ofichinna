namespace Ofichina.Contracts.Requests.Servicos;

/// <summary>
/// Dados necessários para atualização de um serviço.
/// </summary>
public sealed class UpdateServicoRequest : UpdateRequest
{
    /// <summary>
    /// Identificador do serviço.
    /// </summary>
    public Guid ServicoId { get; init; }

    /// <summary>
    /// Nome do serviço.
    /// </summary>
    public string Nome { get; init; } = string.Empty;

    /// <summary>
    /// Descrição do serviço.
    /// </summary>
    public string? Descricao { get; init; }

    /// <summary>
    /// Valor do serviço.
    /// </summary>
    public decimal Valor { get; init; }

    /// <summary>
    /// Indica se o serviço está ativo.
    /// </summary>
    public bool Ativo { get; init; } = true;
}