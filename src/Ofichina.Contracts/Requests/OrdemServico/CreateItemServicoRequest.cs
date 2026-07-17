using Ofichina.Contracts.Requests;

namespace Ofichina.Contracts.Requests.OrdemServico;

/// <summary>
/// Dados necessários para criação de um item de serviço na ordem de serviço.
/// </summary>
public sealed class CreateItemServicoRequest : CreateRequest
{
    /// <summary>
    /// Identificador do serviço cadastrado.
    /// </summary>
    public Guid ServicoId { get; init; }
}
