using Ofichina.Contracts.Common;

namespace Ofichina.Contracts.Responses.Orcamento;

/// <summary>
/// Resposta com os dados de um item previsto do orçamento.
/// </summary>
public sealed class OrcamentoItemResponse : BaseEntity
{
    public Guid OrcamentoId { get; set; }

    public Guid? ServicoId { get; set; }

    public Guid? PecaId { get; set; }

    public int Quantidade { get; set; }
}
