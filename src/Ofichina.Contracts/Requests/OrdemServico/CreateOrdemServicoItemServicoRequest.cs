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
    public Guid OrdemServicoItemId { get; init; }

    /// <summary>
    /// Quantidade do serviço.
    /// </summary>
    public decimal Quantidade { get; init; }
}