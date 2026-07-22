namespace Ofichina.Contracts.Requests.ItemServico;

/// <summary>
/// Dados necessários para criação de um item de serviço na ordem de serviço.
/// </summary>
public sealed class CreateItemServicoRequest : CreateRequest
{
    /// <summary>
    /// Identificador da ordem de serviço.
    /// </summary>
    public Guid OrdemServicoId { get; init; }

    /// <summary>
    /// Identificador da peça vinculada ao serviço.
    /// </summary>
    public Guid PecaServicoId { get; init; }
}
