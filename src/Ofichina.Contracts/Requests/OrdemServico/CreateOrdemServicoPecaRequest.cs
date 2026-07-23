using Ofichina.Contracts.Requests;

namespace Ofichina.Contracts.Requests.OrdensServico;

/// <summary>
/// Peça associada à ordem de serviço.
/// </summary>
public sealed class CreateOrdemServicoPecaRequest : CreateRequest
{
    /// <summary>
    /// Peça cadastrada.
    /// </summary>
    public Guid PecaId { get; init; }

    /// <summary>
    /// Quantidade utilizada.
    /// </summary>
    public int Quantidade { get; init; }
}
