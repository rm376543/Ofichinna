namespace Ofichina.Contracts.Requests.Orcamento;

/// <summary>
/// Requisição para reprovar um orçamento.
/// </summary>
public sealed class ReprovarOrcamentoRequest : BaseRequest
{
    public Guid OrcamentoId { get; set; }
    public string? Motivo { get; init; }

    public ReprovarOrcamentoRequest(Guid orcamentoId, string? motivo)
    {
        OrcamentoId = orcamentoId;
        Motivo = motivo;
    }
}