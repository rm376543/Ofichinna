namespace Ofichina.Contracts.Requests.OrdemServicos;

/// <summary>
/// Peça associada à ordem de serviço.
/// </summary>
public sealed class CreateOrdemServicoItemPecaRequest : CreateRequest
{
    /// <summary>
    /// Peça cadastrada.
    /// </summary>
    public Guid OrdemServicoItemPecaId { get; init; }

    /// <summary>
    /// Quantidade utilizada.
    /// </summary>
    public decimal Quantidade { get; init; }
}