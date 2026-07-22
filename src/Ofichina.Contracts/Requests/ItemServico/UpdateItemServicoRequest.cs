namespace Ofichina.Contracts.Requests.ItemServico;

/// <summary>
/// Dados necessários para atualização de um item de serviço na ordem de serviço.
/// </summary>
public sealed class UpdateItemServicoRequest : UpdateRequest
{
    /// <summary>
    /// Identificador da ordem de serviço.
    /// </summary>
    public Guid OrdemServicoId { get; init; } = Guid.Empty;

    /// <summary>
    /// Identificador do item de serviço.
    /// </summary>
    public Guid ItemServicoId { get; init; } = Guid.Empty;

    /// <summary>
    /// Identificador da peça de serviço vinculada.
    /// </summary>
    public Guid PecaServicoId { get; init; } = Guid.Empty;
}
