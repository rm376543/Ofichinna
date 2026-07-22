using Ofichina.Contracts.Requests;

namespace Ofichina.Contracts.Requests.OrdemServico;

/// <summary>
/// Dados necessários para adicionar uma peça a um item de serviço.
/// </summary>
public sealed class CreateItemServicoPecaRequest : CreateRequest
{
    /// <summary>
    /// Identificador da peça cadastrada.
    /// </summary>
    public Guid PecaId { get; init; }

    /// <summary>
    /// Quantidade utilizada.
    /// </summary>
    public int Quantidade { get; init; }
}