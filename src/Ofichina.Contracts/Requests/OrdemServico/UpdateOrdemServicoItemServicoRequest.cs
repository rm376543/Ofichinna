namespace Ofichina.Contracts.Requests.OrdemServicos;

/// <summary>
/// Serviço atualizado na ordem de serviço.
/// </summary>
public sealed class UpdateOrdemServicoItemServicoRequest : UpdateRequest
{
    /// <summary>
    /// Identificador do item da ordem.
    /// 
    /// Null quando for um novo serviço.
    /// </summary>
    public Guid? OrdemServicoItemServicoId { get; init; }

    /// <summary>
    /// Serviço cadastrado.
    /// </summary>
    public Guid ServicoId { get; init; }

    /// <summary>
    /// Quantidade.
    /// </summary>
    public decimal Quantidade { get; init; }
}