using Ofichina.Contracts.Requests;

namespace Ofichina.Contracts.Requests.OrdemServico;

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