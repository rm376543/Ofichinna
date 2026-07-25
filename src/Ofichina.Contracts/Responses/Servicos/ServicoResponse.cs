using Ofichina.Contracts.Common;

namespace Ofichina.Contracts.Responses.Servicos;

/// <summary>
/// Resposta com os dados completos de um serviço.
/// </summary>
public sealed class ServicoResponse : BaseEntity
{
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
}