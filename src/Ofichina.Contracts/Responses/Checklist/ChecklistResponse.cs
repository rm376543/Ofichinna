using Ofichina.Contracts.Common;

namespace Ofichina.Contracts.Responses.Checklist;

/// <summary>
/// Resposta com os dados do checklist de entrada do veículo.
/// </summary>
public sealed class ChecklistResponse : BaseResponse
{
    public Guid ChecklistId { get; set; }

    public Guid AgendamentoId { get; set; }

    public string ItensVerificados { get; set; } = string.Empty;

    public string? Observacoes { get; set; }

    public bool Finalizado { get; set; }
}
