namespace Ofichina.Contracts.Requests.Orcamentos;

/// <summary>
/// Serviço atualizado no orçamento.
/// </summary>
public sealed class UpdateOrcamentoServicoRequest : UpdateRequest
{
    /// <summary>
    /// Serviço.
    /// </summary>
    public Guid ServicoId { get; init; }

    /// <summary>
    /// Peças associadas ao serviço.
    /// </summary>
    public ICollection<UpdateOrcamentoServicoPecaRequest> Pecas { get; init; } = [];
}