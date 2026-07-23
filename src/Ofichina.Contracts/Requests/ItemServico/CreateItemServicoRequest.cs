namespace Ofichina.Contracts.Requests.ItensServico;

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
    /// Peças vinculadas ao item.
    /// </summary>
    public IReadOnlyCollection<CreateItemServicoPecaRequest> Pecas { get; init; } = [];
}

/// <summary>
/// Peça vinculada ao item de serviço.
/// </summary>
public sealed class CreateItemServicoPecaRequest
{
    public Guid ServicoPecaId { get; init; }
    public int Quantidade { get; init; }
}
