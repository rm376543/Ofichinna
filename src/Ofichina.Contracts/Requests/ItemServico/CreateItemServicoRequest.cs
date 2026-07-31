namespace Ofichina.Contracts.Requests.ItensServico;

/// <summary>
/// Dados necessários para criação de um item de serviço na ordem de serviço.
/// </summary>
public sealed class CreateItemServicoRequest : CreateRequest
{
    /// <summary>
    /// Identificador da ordem de serviço.
    /// </summary>
    public Guid OrdemServicoId { get; init; }

    /// <summary>
    /// Serviço executado na ordem.
    /// </summary>
    public Guid ServicoId { get; init; }

    /// <summary>
    /// Peça utilizada no serviço.
    /// </summary>
    public Guid PecaId { get; init; }

    /// <summary>
    /// Quantidade de peças utilizadas.
    /// </summary>
    public int Quantidade { get; init; }
}
