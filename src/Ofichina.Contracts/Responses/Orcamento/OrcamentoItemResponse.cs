using Ofichina.Contracts.Common;

namespace Ofichina.Contracts.Responses.Orcamento;

/// <summary>
/// Resposta com os dados de um item de serviço previsto do orçamento.
/// </summary>
public sealed class OrcamentoItemResponse : BaseEntity
{
    public Guid OrcamentoId { get; set; }

    public Guid ServicoId { get; set; }

    public string Descricao { get; set; } = string.Empty;

    public decimal ValorServico { get; set; }

    public decimal ValorTotal { get; set; }

    public ICollection<OrcamentoItemServicoPecaResponse> Pecas { get; set; } = [];
}

/// <summary>
/// Resposta com os dados de uma peça vinculada a um item de serviço do orçamento.
/// </summary>
public sealed class OrcamentoItemServicoPecaResponse
{
    public Guid PecaId { get; set; }

    public string Descricao { get; set; } = string.Empty;

    public int Quantidade { get; set; }

    public decimal ValorUnitario { get; set; }

    public decimal ValorTotal { get; set; }
}
