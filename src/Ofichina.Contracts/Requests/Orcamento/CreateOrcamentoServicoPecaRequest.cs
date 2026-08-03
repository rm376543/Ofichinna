using Ofichina.Contracts.Requests;

namespace Ofichina.Contracts.Requests.Orcamentos;

/// <summary>
/// Peça associada a um item de serviço do orçamento.
/// </summary>
public sealed class CreateOrcamentoServicoPecaRequest : CreateRequest
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