using Ofichina.Contracts.Common;

namespace Ofichina.Contracts.Responses.Orcamento;

/// <summary>
/// Resposta com os dados de um orçamento.
/// </summary>
public sealed class OrcamentoResponse : BaseResponse
{
    public Guid OrcamentoId { get; set; }

    public Guid PessoaId { get; set; }

    public Guid VeiculoId { get; set; }

    public Guid AgendamentoId { get; set; }

    public Guid MecanicoDiagnosticoId { get; set; }

    public Guid ResponsavelId { get; set; }

    public DateTime DataValidade { get; set; }

    public decimal Desconto { get; set; }

    public string? Observacoes { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime DataCriacao { get; set; }

    public decimal ValorTotal { get; set; }

    public ICollection<OrcamentoItemResponse> ItensServico { get; set; } = [];
}
