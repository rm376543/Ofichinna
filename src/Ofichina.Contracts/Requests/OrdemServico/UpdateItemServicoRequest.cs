using Ofichina.Contracts.Requests;

namespace Ofichina.Contracts.Requests.OrdemServico;

/// <summary>
/// Dados necessários para atualização de um item de serviço na ordem de serviço.
/// </summary>
public sealed class UpdateItemServicoRequest : UpdateRequest
{
    /// <summary>
    /// Descrição atualizada do serviço.
    /// </summary>
    public string Descricao { get; init; } = string.Empty;

    /// <summary>
    /// Novo valor cobrado pelo serviço.
    /// </summary>
    public decimal Valor { get; init; }
}
