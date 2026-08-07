using Ofichina.Contracts.Common;

namespace Ofichina.Contracts.Responses.Orcamento;

/// <summary>
/// Resposta simplificada para listagem de orçamentos.
/// </summary>
public sealed class OrcamentoSimplesResponse : BaseResponse
{
    public Guid OrcamentoSimplesId { get; set; }

    public string Cliente { get; set; } = string.Empty;

    public string Responsavel { get; set; } = string.Empty;

    public string MecanicoDiagnostico { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string DataCriacao { get; set; } = string.Empty;

    public string DataValidade { get; set; } = string.Empty;

    public decimal Desconto { get; set; }

    public decimal ValorTotal { get; set; }
}
