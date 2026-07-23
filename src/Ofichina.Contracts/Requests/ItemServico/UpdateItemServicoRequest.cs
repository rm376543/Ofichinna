namespace Ofichina.Contracts.Requests.ItensServico;

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
    /// Peças vinculadas ao item.
    /// </summary>
    public IReadOnlyCollection<UpdateItemServicoPecaRequest> Pecas { get; init; } = [];
}

/// <summary>
/// Peça vinculada ao item de serviço.
/// </summary>
public sealed class UpdateItemServicoPecaRequest
{
    public Guid PecaServicoId { get; init; }
    public int Quantidade { get; init; }
}
