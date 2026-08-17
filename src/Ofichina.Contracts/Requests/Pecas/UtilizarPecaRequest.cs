namespace Ofichina.Contracts.Requests.Pecas;

/// <summary>
/// Classe de requisicao para atualizar uma peca de um item de servico especifico.
/// </summary>
public sealed class UtilizarPecaRequest
{
    /// <summary>
    /// Identificador da ordem de serviço.
    /// </summary>
    public Guid OrdemServicoId { get; init; }

    /// <summary>
    /// Identificador do item de serviço.
    /// </summary>
    public Guid ItemServicoId { get; init; }

    /// <summary>
    /// Identificador da peça vinculada ao serviço.
    /// </summary>
    public Guid PecaId { get; init; }
};