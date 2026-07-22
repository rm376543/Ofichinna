using Ofichina.Contracts.Requests;

namespace Ofichina.Contracts.Requests.Servicos;

/// <summary>
/// Dados para adicionar uma peça a um serviço.
/// </summary>
public sealed class CreateServicoPecaRequest : CreateRequest
{
    /// <summary>
    /// Identificador do serviço.
    /// </summary>
    public Guid ServicoId { get; init; }

    /// <summary>
    /// Identificador da peça.
    /// </summary>
    public Guid PecaId { get; init; }

    /// <summary>
    /// Quantidade da peça no serviço.
    /// </summary>
    public int Quantidade { get; init; }
}