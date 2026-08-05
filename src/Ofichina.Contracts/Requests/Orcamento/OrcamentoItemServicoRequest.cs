namespace Ofichina.Contracts.Requests.Orcamento;

/// <summary>
/// Item de serviço previsto no orçamento.
/// </summary>
public sealed class OrcamentoItemServicoRequest
{
    /// <summary>
    /// Identificador do serviço previsto.
    /// </summary>
    public Guid ServicoId { get; init; }

    /// <summary>
    /// Identificador da peça prevista.
    /// </summary>
    public Guid PecaId { get; init; }

    /// <summary>
    /// Quantidade de peças previstas.
    /// </summary>
    public int Quantidade { get; init; }
}
