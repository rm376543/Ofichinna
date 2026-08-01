using Ofichina.Contracts.Common;

namespace Ofichina.Contracts.Responses.Orcamento;

/// <summary>
/// Resposta com os dados do checklist de entrada do veículo.
/// </summary>
public sealed class ChecklistResponse : BaseEntity
{
    public Guid OrcamentoId { get; set; }

    public int HodometroEntrada { get; set; }

    public string ItensVerificados { get; set; } = string.Empty;

    public string? Observacoes { get; set; }
}
