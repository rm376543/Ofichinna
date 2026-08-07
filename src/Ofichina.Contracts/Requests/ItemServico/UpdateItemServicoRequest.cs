namespace Ofichina.Contracts.Requests.ItensServico;

/// <summary>
/// Dados necessários para atualização de um item de serviço na ordem de serviço.
/// </summary>
public sealed class UpdateItemServicoRequest : UpdateRequest
{
    /// <summary>
    /// Identificador do item de serviço.
    /// </summary>
    public Guid ItemServicoId { get; init; } = Guid.Empty;

    /// <summary>
    /// Identificador da ordem de serviço.
    /// </summary>
    public Guid OrdemServicoId { get; init; } = Guid.Empty;

    /// <summary>
    /// Identificador do serviço executado.
    /// </summary>
    public Guid ServicoId { get; init; } = Guid.Empty;

    /// <summary>
    /// Identificador da peça utilizada.
    /// </summary>
    public Guid PecaId { get; init; } = Guid.Empty;

    /// <summary>
    /// Quantidade de peças utilizadas.
    /// </summary>
    public int Quantidade { get; init; }
}

