using Ofichina.Contracts.Common;

namespace Ofichina.Contracts.Requests.Orcamento;

/// <summary>
/// Requisição para aprovar um orçamento e gerar a ordem de serviço.
/// </summary>
public sealed class AprovarOrcamentoRequest : BaseRequest
{
    public Guid OrcamentoId { get; init; }

    public int Hodometro { get; init; }

    public AprovarOrcamentoRequest(Guid orcamentoId, int hodometro)
    {
        OrcamentoId = orcamentoId;
        Hodometro = hodometro;
    }
}
