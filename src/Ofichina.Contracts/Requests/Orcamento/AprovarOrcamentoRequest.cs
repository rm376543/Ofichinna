namespace Ofichina.Contracts.Requests.Orcamento;

/// <summary>
/// Requisição para aprovar um orçamento e gerar a ordem de serviço.
/// </summary>
public sealed class AprovarOrcamentoRequest
{
    public Guid OrcamentoId { get; init; }

    public AprovarOrcamentoRequest(Guid orcamentoId)
    {
        OrcamentoId = orcamentoId;
    }
}
