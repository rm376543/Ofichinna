using Ofichina.Contracts.Requests;

namespace Ofichina.Contracts.Requests.OrdemServico;

/// <summary>
/// Serviço associado à ordem de serviço.
/// </summary>
public sealed class CreateOrdemServicoItemServicoRequest : CreateRequest
{
    /// <summary>
    /// Serviço cadastrado.
    /// </summary>
    public Guid ServicoId { get; init; }

    /// <summary>
    /// Peças associadas ao serviço.
    /// </summary>
    public ICollection<CreateOrdemServicoPecaRequest> Pecas { get; init; } = [];
}