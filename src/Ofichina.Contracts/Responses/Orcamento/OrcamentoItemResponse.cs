using Ofichina.Contracts.Responses.OrdemServico;

namespace Ofichina.Contracts.Responses.Orcamento;

/// <summary>
/// Resposta com os dados de um item de orçamento.
/// </summary>
public sealed class OrcamentoItemResponse
{
    public Guid OrcamentoId { get; set; }

    public List<ServicoItemResponse> Servicos { get; set; } = [];
}
