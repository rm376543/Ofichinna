using Ofichina.Contracts.Requests;

namespace Ofichina.Contracts.Requests.Orcamento;

/// <summary>
/// Requisição para reprovar um orçamento.
/// </summary>
public sealed class ReprovarOrcamentoRequest : BaseRequest
{
    public string? Motivo { get; init; }
}