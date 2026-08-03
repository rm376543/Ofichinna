using Ofichina.Contracts.Requests;

namespace Ofichina.Contracts.Requests.Orcamentos;

/// <summary>
/// Item de serviço do orçamento.
/// </summary>
public sealed class CreateOrcamentoServicoRequest : CreateRequest
{
    /// <summary>
    /// Serviço cadastrado.
    /// </summary>
    public Guid ServicoId { get; init; }

    /// <summary>
    /// Peças associadas ao serviço.
    /// </summary>
    public ICollection<CreateOrcamentoServicoPecaRequest> Pecas { get; init; } = [];
}
