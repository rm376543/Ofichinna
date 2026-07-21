using Ofichina.Contracts.Requests;

namespace Ofichina.Contracts.Requests.OrdemServico;

/// <summary>
/// Peça atualizada na ordem de serviço.
/// </summary>
public sealed class UpdateOrdemServicoPecaRequest : UpdateRequest
{
    /// <summary>
    /// Peça cadastrada.
    /// </summary>
    public Guid PecaId { get; init; }

    /// <summary>
    /// Quantidade.
    /// </summary>
    public int Quantidade { get; init; }
}
