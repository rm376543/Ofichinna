using Ofichina.Contracts.Requests;

namespace Ofichina.Contracts.Requests.OrdemServico;

/// <summary>
/// Dados necessários para criação de um item de serviço na ordem de serviço.
/// </summary>
public sealed class CreateItemServicoRequest : CreateRequest
{
    /// <summary>
    /// Descrição do serviço executado ou previsto.
    /// </summary>
    public string Descricao { get; init; } = string.Empty;

    /// <summary>
    /// Valor cobrado pelo serviço.
    /// </summary>
    public decimal Valor { get; init; }
}
