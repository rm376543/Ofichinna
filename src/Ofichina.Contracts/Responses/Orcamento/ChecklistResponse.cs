using Ofichina.Contracts.Common;

namespace Ofichina.Contracts.Responses.Orcamento;

/// <summary>
/// Resposta com os dados do checklist de entrada do veículo.
/// </summary>
public sealed class ChecklistResponse : BaseEntity
{
    public Guid ChecklistId { get; set; }

    public Guid AgendamentoId { get; set; }

    public Guid VeiculoId { get; set; }

    public Guid PessoaId { get; set; }

    public int HodometroEntrada { get; set; }

    public string ItensVerificados { get; set; } = string.Empty;

    public string? Observacoes { get; set; }

    public bool Finalizado { get; set; }
}
