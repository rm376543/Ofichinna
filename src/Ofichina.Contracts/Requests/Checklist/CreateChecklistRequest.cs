using Ofichina.Contracts.Requests;

namespace Ofichina.Contracts.Requests.Checklist;

/// <summary>
/// Requisição para criação de checklist.
/// </summary>
public sealed class CreateChecklistRequest : CreateRequest
{
    public Guid AgendamentoId { get; init; }

    public Guid VeiculoId { get; init; }

    public Guid PessoaId { get; init; }

    public int HodometroEntrada { get; init; }

    public string ItensVerificados { get; init; } = string.Empty;

    public string? Observacoes { get; init; }
}