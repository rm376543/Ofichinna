using Ofichina.Contracts.Common;

namespace Ofichina.Contracts.Requests.ItensServico;

/// <summary>
/// Dados necessários para criação de um item de serviço somente-serviço na ordem de serviço.
/// </summary>
public sealed class CreateServicoOrdemServicoRequest : CreateRequest
{
    /// <summary>
    /// Identificador da ordem de serviço.
    /// </summary>
    public Guid OrdemServicoId { get; init; }

    /// <summary>
    /// Identificador do serviço executado.
    /// </summary>
    public Guid ServicoId { get; init; }
}