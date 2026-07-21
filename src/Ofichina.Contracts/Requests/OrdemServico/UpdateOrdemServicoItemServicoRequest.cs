using Ofichina.Contracts.Requests;

namespace Ofichina.Contracts.Requests.OrdemServico;

/// <summary>
/// Serviço atualizado na ordem de serviço.
/// </summary>
public sealed class UpdateOrdemServicoItemServicoRequest : UpdateRequest
{
    /// <summary>
    /// Serviço cadastrado.
    /// </summary>
    public Guid ServicoId { get; init; }

    /// <summary>
    /// Peças associadas ao serviço.
    /// </summary>
    public ICollection<UpdateOrdemServicoPecaRequest> Pecas { get; init; } = [];
}